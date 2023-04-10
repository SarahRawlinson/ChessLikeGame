using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using LibObjects;
using Multiplayer.View.DisplayData;
using NetClient;
using UnityEngine;
using UnityEngine.Serialization;


namespace Multiplayer.Controllers
{
    public class WebSocketConnection : MonoBehaviour
    {
        private const string GameRoomMeta = "ChessGameRoom";
        private const string ChatRoomMeta = "ChatApp";
        public static event Action<(User user, string message)> onMessageRecieved;
        public static event Action<bool> onAuthenicate;
        public static event Action<User> onUserLoggedIn;
        public static event Action<User> onUserDisconnect;
        public static event Action<List<User>> onUsersList;
        public static event Action<List<Room>>onGameRoomList;
        public static event Action<List<Room>>onChatRoomList;
        public static event Action<Room> onHostGame;
        public static event Action<Room> onHostChat;
        public static event Action<Room> onJoinedChat;
        public static event Action<Room> onJoinedGame;
        public static event Action<Room> onLeftChat;
        public static event Action<Room> onLeftGame;
        public static event Action<(User user, Guid roomGuid)> onUserJoinedGameRoom;
        public static event Action<(User user, Guid roomGuid)> onUserLeftGameRoom;
        public static event Action<(User user, Guid roomGuid)> onUserJoinedChatRoom;
        public static event Action<(User user, Guid roomGuid)> onUserLeftChatRoom;
        
        
        public static event Action<Room> onRecievedYouWhereRemovedFromTheRoom;
        public static event Action<Room> onRecievedYouHaveBeenBannedFromRoom;
        public static event Action<Room> onRecievedYouAreNoLongerBannedFromRoom;
        public static event Action<Room> onRecievedYouHaveBeenApprovedForRoom;
        public static event Action<Room> onRecievedYouAreNoLongerApprovedForRoom;
        
        
        public static event Action<(Room room, User user, string Message)> onChatRoomMessageRecieved;
        public static event Action<(Room room, User user, string Message)> onGameRoomMessageRecieved;
        public static event Action<(Room room, User user, string Message)> onChatRoomPrivateMessageRecieved;
        public static event Action<(Room room, User user, string Message)> onGameRoomPrivateMessageRecieved;
        public static event Action<(User user, string messageSent)> onReceivedCommunicationToAll;
        public static event Action<Room> onRoomDestroyed;
        public static event Action<(Room room, List<User> users)> onReceivedUsersListInRoom;
        public static event Action<(Room room, List<User> users)> onReceivedApprovedUsersListInRoom;
        public static event Action<(Room room, List<User> users)> onReceivedBannedUsersListInRoom;
        public static event Action<User> onSetUser;
        
        public static event Action onLoggedOff ;
        public static event Action<(CommunicationTypeEnum comEnum, string message)> onOnErrorMessageReceived;

        [SerializeField] private bool refreshSubscribed = false;
        [SerializeField] private float refreshTime = 10f;
        [SerializeField] private string url = "localhost";
        [SerializeField] private int port = 8080;
        public User user { get; set; }
        private Guid clientID = Guid.Empty;
        private bool refresh;
        private ClientWebSocket webSocket;
        private readonly Client client = new Client();
        private Dictionary<Guid,Room> roomsDictionary = new Dictionary<Guid, Room>();

        private void Start()
        {
            client.SetDisconnectOnFailAuthentication(true);
            client.onMessageFromUser += MessageReceived;
            client.onAuthenticate += Authentication;
            client.onRoomDestroyed += RoomDestroyed;
            client.onUserList += UserListReceived;
            client.onUserJoinedRoom += UserJoinedRoom;
            client.onUserLeftRoom += UserLeftRoom;
            client.onRoomList += RoomListReceived;
            client.onRoomCreated += CreatedRoom;
            client.onRemovedFromRoom += RemovedFromTheRoom;
            client.onBannedFromRoom += BannedFromTheRoom;
            client.onNoLongerBannedFromRoom += NoLongerBannedFromRoom;
            client.onApprovedForRoom += ApprovedForRoom;
            client.onNoLongerApprovedForRoom += NoLongerApprovedForRoom;
            client.onRoomJoined += JoinedRoom;
            client.onRoomLeft += RoomLeft;
            client.onRoomMessage += RoomMessageReceived;
            client.onUsersListInRoom += UsersInRoom;
            client.onMessageWasReceivedByUser += ReceivedMessageWasReceivedByUser;
            client.onCommunicationToAll += ReceivedCommunicationToAll;
            client.onErrorResponseFromServer += ReceivedErrorResponseFromServer;
            client.onRecievedUserWithGuidEvent += UserWithGuidReceived;
            client.onIncomingWebSocketMessageEvent += LogMessageReceived;
            client.onRecievedGuid += ClientGuidReceived;
            client.onMessageSentToSocketEvent += LogMessageSent;
            client.onApprovedUsersListInRoom += ApprovedUsersInRoom;
            client.onBannedUsersListInRoom += BannedUsersInRoom;
            client.onUserLoggedIn += UserLoggedIn;
            client.onUserDisconnected += UserDisconnected;
            client.onPrivateRoomMessage += PrivateRoomMessage;
        }

        private void PrivateRoomMessage((Room room, User user, string Message) obj)
        {
            Room objRoom = obj.room;
            if (IsChatRoom(objRoom))
            {
                Debug.Log($"PrivateChatRoomMessage={obj.user.GetUserName()} {objRoom.GetRoomName()} {obj.Message}");
                onChatRoomPrivateMessageRecieved?.Invoke(obj);
            }
            if (IsGameRoom(objRoom))
            {
                Debug.Log($"PrivateGameRoomMessage={obj.user.GetUserName()} {objRoom.GetRoomName()} {obj.Message}");
                onGameRoomPrivateMessageRecieved?.Invoke(obj);
            }
        }

        private void UserDisconnected(User obj)
        {
            Debug.Log($"UserDisconnected={obj.GetUserName()}");
            onUserDisconnect?.Invoke(obj);
        }

        private void UserLoggedIn(User obj)
        {
            Debug.Log($"UserLoggedIn={obj.GetUserName()}");
            onUserLoggedIn?.Invoke(obj);
        }


        private void BannedUsersInRoom((Room room, List<User> users) obj)
        {
            Debug.Log($"BannedUsersInRoom={obj.room.GetRoomName()}, count={obj.users.Count}");
            onReceivedBannedUsersListInRoom?.Invoke(obj);
        }

        private void ApprovedUsersInRoom((Room room, List<User> users) obj)
        {
            Debug.Log($"ApprovedUsersInRoom={obj.room.GetRoomName()}, count={obj.users.Count}");
            onReceivedApprovedUsersListInRoom?.Invoke(obj);
        }


        private void RoomLeft(Room obj)
        {
            if (IsChatRoom(obj))
            {
                Debug.Log($"ChatRoomLeft={obj.GetRoomName()}");
                onLeftChat?.Invoke(obj);
            }
            if (IsGameRoom(obj))
            {
                Debug.Log($"GameRoomLeft={obj.GetRoomName()}");
                onLeftGame?.Invoke(obj);
            }
        }

        private void NoLongerApprovedForRoom(Room obj)
        {
            Debug.Log($"NoLongerApprovedForRoom={obj.GetRoomName()}");
            onRecievedYouAreNoLongerApprovedForRoom?.Invoke(obj);
        }

        private void ApprovedForRoom(Room obj)
        {
            Debug.Log($"ApprovedForRoom={obj.GetRoomName()}");
            onRecievedYouHaveBeenApprovedForRoom?.Invoke(obj);
        }

        private void NoLongerBannedFromRoom(Room obj)
        {
            Debug.Log($"NoLongerBannedFromRoom={obj.GetRoomName()}");
            onRecievedYouAreNoLongerBannedFromRoom?.Invoke(obj);
        }

        private void BannedFromTheRoom(Room obj)
        {
            Debug.Log($"BannedFromTheRoom={obj.GetRoomName()}");
            onRecievedYouHaveBeenBannedFromRoom?.Invoke(obj);
        }

        private void RemovedFromTheRoom(Room obj)
        {
            Debug.Log($"RemovedFromTheRoom={obj.GetRoomName()}");
            onRecievedYouWhereRemovedFromTheRoom?.Invoke(obj);
        }

        public User GetClientUser()
        {
            return user;
        }

        private void ReceivedErrorResponseFromServer((CommunicationTypeEnum comEnum, string message) obj)
        {
            Debug.Log($"Error={obj.comEnum.ToString()}, Message={obj.message}");
            onOnErrorMessageReceived?.Invoke(obj);
        }

        private void ReceivedMessageWasReceivedByUser((User user, string messageSent) obj)
        {
            Debug.Log($"ReceivedMessageWasReceivedByUser={obj.user.GetUserName()}, message={obj.messageSent}");
        }

        private void ReceivedCommunicationToAll((User user, string messageSent) obj)
        {
            Debug.Log($"ReceivedCommunicationToAll=from={obj.user.GetUserName()}, message={obj.messageSent}");
            onReceivedCommunicationToAll?.Invoke(obj);
        }

        private void UsersInRoom((Room room, List<User> users) obj)
        {
            Debug.Log($"UsersInRoom={obj.room.GetRoomName()}, count={obj.users.Count}");
            onReceivedUsersListInRoom?.Invoke(obj);
        }

        private void RoomDestroyed(Room obj)
        {
            Debug.Log($"RoomDestroyed={obj.GetRoomName()}");
            onRoomDestroyed?.Invoke(obj);
        }

        private void UserWithGuidReceived((User user, Guid guid) obj)
        {
            Debug.Log($"UserWithGuidReceived={obj.user.GetUserName()} {obj.guid}");
            if (obj.guid == clientID)
            {
                user = obj.user;
            }
        }

        private void RoomMessageReceived((Room room, User user, string Message) obj)
        {
            Room objRoom = obj.room;
            if (IsChatRoom(objRoom))
            {
                Debug.Log($"ChatRoomMessageReceived={obj.user.GetUserName()} {objRoom.GetRoomName()} {obj.Message}");
                onChatRoomMessageRecieved?.Invoke(obj);
            }
            if (IsGameRoom(objRoom))
            {
                Debug.Log($"GameRoomMessageReceived={obj.user.GetUserName()} {objRoom.GetRoomName()} {obj.Message}");
                onGameRoomMessageRecieved?.Invoke(obj);
            }
        }

        private static bool IsGameRoom(Room objRoom)
        {
            return objRoom.GetMeta().Contains(GameRoomMeta);
        }

        private static bool IsChatRoom(Room objRoom)
        {
            return objRoom.GetMeta().Contains(ChatRoomMeta);
        }

        private void JoinedRoom(Room obj)
        {
            Debug.Log($"JoinedRoom={obj.GetRoomName()}");
            if (IsChatRoom(obj))
            {
                Debug.Log($"onJoinedChat={obj.GetRoomName()}");
                onJoinedChat?.Invoke(obj);
            }
            if (IsGameRoom(obj))
            {
                Debug.Log($"onJoinedGame={obj.GetRoomName()}");
                onJoinedGame?.Invoke(obj);
            }
        }

        private void CreatedRoom(Room obj)
        {
            Debug.Log($"CreatedRoom={obj.GetRoomName()}");
            if (IsChatRoom(obj))
            {
                onHostChat?.Invoke(obj);
            }
            if (IsGameRoom(obj))
            {
                onHostGame?.Invoke(obj);
            }
        }

        private void LogMessageReceived(string obj)
        {
            Debug.Log($"WEB SOCKET RECEIVED MESSAGE {obj}");
        }
        
        private void LogMessageSent(string obj)
        {
            Debug.Log($"WEB SOCKET SENT MESSAGE {obj}");
        }

        private void ClientGuidReceived(Guid obj)
        {
            Debug.Log($"ClientGuidReceived={obj}");
            clientID = obj;
        }

        private void RoomListReceived(List<Room> obj)
        {
            Debug.Log($"RoomListReceived={obj.Count}");
            
            roomsDictionary.Clear();
            List<Room> chatRooms = new List<Room>();
            List<Room> gameRooms = new List<Room>();
            foreach (var room in obj)
            {
                roomsDictionary.Add(room.GetGuid(), room);
                if (IsChatRoom(room))
                {
                    chatRooms.Add(room);
                }
                if (IsGameRoom(room))
                {
                    gameRooms.Add(room);
                }
            }
            onGameRoomList?.Invoke(gameRooms);
            onChatRoomList?.Invoke(chatRooms);
        }

        private void UserLeftRoom((User user, Guid roomGuid) obj)
        {
            Debug.Log($"UserLeftRoom={obj}");
            if (!roomsDictionary.ContainsKey(obj.roomGuid))
            {
                return;
            }
            if (IsChatRoom(roomsDictionary[obj.roomGuid]))
            {
                onUserLeftChatRoom?.Invoke(obj);
            }
            if (IsGameRoom(roomsDictionary[obj.roomGuid]))
            {
                onUserLeftGameRoom?.Invoke(obj);
            }
        }

        private void UserJoinedRoom((User user, Guid roomGuid) obj)
        {
            Debug.Log($"UserJoinedRoom={obj.user.GetUserName()}");
            if (!roomsDictionary.ContainsKey(obj.roomGuid))
            {
                return;
            }
            if (IsChatRoom(roomsDictionary[obj.roomGuid]))
            {
                onUserJoinedChatRoom?.Invoke(obj);
            }
            if (IsGameRoom(roomsDictionary[obj.roomGuid]))
            {
                onUserJoinedGameRoom?.Invoke(obj);
            }
        }

        private static void UserListReceived(List<User> obj)
        {
            Debug.Log($"UserListReceived={obj.Count}");
            onUsersList?.Invoke(obj);
        }

        private static void MessageReceived((User user, string message) obj)
        {
            Debug.LogWarning($"MessageReceived=From={obj.user.GetUserName()}/Message={obj.message}");
            onMessageRecieved?.Invoke(obj);
        }

        private void Authentication(bool obj)
        {
            Debug.Log($"Authentication={obj}");
            if (!obj)
            {
                Task.FromResult(CloseSocket());
            }
            else
            {
                StartCoroutine(nameof(RefreshSubscribed));
            }
            onAuthenicate?.Invoke(obj);
        }

        // Start is called before the first frame update
        public async Task Connect(string userName, string password)
        {
            Debug.Log("login");
            // await client.Connect();
            // await client.Connect("192.168.1.156","8080");
            await client.Connect(url,port.ToString());
            Task.FromResult(StartConnection());
            await client.RequestAuthenticate(userName, password);
        }

        public async Task StartConnection()
        {
            await client.Listen();
            Debug.Log("Stopped Listening");
        }

        public void RefreshRooms()
        {
            client.RequestRoomList();
        }
        
        public void RefreshUsers()
        {
            client.RequestUserList();
        }

        public IEnumerator RefreshSubscribed()
        {
            if (!refreshSubscribed) yield break;
            if (refresh) yield break;
            refresh = true;
            while (refresh)
            {
                yield return new WaitForSeconds(1f);
                if (!client.RequestRoomList())
                {
                    Debug.Log("couldnt refresh room list");
                }
                yield return new WaitForSeconds(1f);
                
                if (!client.RequestUserList())
                {
                    Debug.Log("couldn't refresh user list");
                }
                yield return new WaitForSeconds(refreshTime);
            }
            Debug.Log("Stopped Asking Server For Updates");
        }
        
        public void SendMessageToUser(User user, string Message)
        {
            client.RequestSendMessageToUser(user, Message);
        }

        public void RefreshRoomList()
        {
            Debug.Log("asked for rooms");
            client.RequestRoomList();
        }
        
        public void CreateNewGameRoom(int roomSize, bool isPublic, string nameOfRoom)
        {
            client.RequestCreateRoom(GameRoomMeta, roomSize, isPublic, nameOfRoom);
        }
        
        public void CreateNewChatRoom(int roomSize, bool isPublic, string nameOfRoom)
        {
            client.RequestCreateRoom(ChatRoomMeta, roomSize, isPublic, nameOfRoom);
        }

        // Close the WebSocket connection when the script is destroyed
        private async void OnDestroy()
        {
            client.onMessageFromUser -= MessageReceived;
            client.onAuthenticate -= Authentication;
            client.onRoomDestroyed -= RoomDestroyed;
            client.onUserList -= UserListReceived;
            client.onUserJoinedRoom -= UserJoinedRoom;
            client.onUserLeftRoom -= UserLeftRoom;
            client.onRoomList -= RoomListReceived;
            client.onRoomCreated -= CreatedRoom;
            client.onRemovedFromRoom -= RemovedFromTheRoom;
            client.onBannedFromRoom -= BannedFromTheRoom;
            client.onNoLongerBannedFromRoom -= NoLongerBannedFromRoom;
            client.onApprovedForRoom -= ApprovedForRoom;
            client.onNoLongerApprovedForRoom -= NoLongerApprovedForRoom;
            client.onRoomJoined -= JoinedRoom;
            client.onRoomLeft -= RoomLeft;
            client.onRoomMessage -= RoomMessageReceived;
            client.onUsersListInRoom -= UsersInRoom;
            client.onMessageWasReceivedByUser -= ReceivedMessageWasReceivedByUser;
            client.onCommunicationToAll -= ReceivedCommunicationToAll;
            client.onErrorResponseFromServer -= ReceivedErrorResponseFromServer;
            client.onRecievedUserWithGuidEvent -= UserWithGuidReceived;
            client.onIncomingWebSocketMessageEvent -= LogMessageReceived;
            client.onRecievedGuid -= ClientGuidReceived;
            client.onMessageSentToSocketEvent -= LogMessageSent;
            client.onApprovedUsersListInRoom -= ApprovedUsersInRoom;
            client.onBannedUsersListInRoom -= BannedUsersInRoom;
            client.onUserLoggedIn -= UserLoggedIn;
            client.onUserDisconnected -= UserDisconnected;
            await CloseSocket();
        }
        
        public void CloseConnection()
        {
            Task.FromResult(CloseSocket());
        }
        
        private async Task CloseSocket()
        {
            
            refresh = false;
            client.Disconnect();
        }

        public void SendMessageToRoom(Room room, string objMessage)
        {
            client.RequestSendMessageToRoomAsync(room.GetGuid(), objMessage);
        }
        
        public void SendMessageToUserInRoom(Room room, string objMessage, User user)
        {
            client.RequestSendPrivateMessageToUserInRoomAsync(room.GetGuid(), objMessage, user.GetUserGuid());
        }

        public void JoinRoom(Guid room)
        {
            client.RequestToAddUserToRoom(user, room);
        }

        public void AskForUsers(Room room)
        {
            Task.FromResult(client.RequestGetUsersInRoomAsync(room.GetGuid()));
        }

        public void LeaveRoom(Room room)
        {
            client.RequestRemoveUserFromRoom(room, user);
        }

        public void ApproveUser(User selectedUser, Room windowInfoRoom)
        {
            client.RequestApproveUserFromRoom(windowInfoRoom, selectedUser);
        }
        
        public void RemoveApproveUser(User selectedUser, Room windowInfoRoom)
        {
            client.RequestRemoveApproveFromUserInRoom(windowInfoRoom, selectedUser);
        }
        
        public void BanUser(User selectedUser, Room windowInfoRoom)
        {
            client.RequestBanUserFromRoom(windowInfoRoom, selectedUser);
        }
        
        public void UnbanUser(User selectedUser, Room windowInfoRoom)
        {
            client.RequestRemoveBanFromUserInRoom(windowInfoRoom, selectedUser);
        }
        
        public void AddUser(User selectedUser, Room windowInfoRoom)
        {
            client.RequestToAddUserToRoom(selectedUser, windowInfoRoom.GetGuid());
        }
        
        public void RemoveUser(User selectedUser, Room windowInfoRoom)
        {
            client.RequestRemoveUserFromRoom(windowInfoRoom, selectedUser);
        }
        
        public void GetBannedUsers(Room room)
        {
            client.RequestGetBannedUsersInRoomAsync(room.GetGuid());
        }
        
        public void GetApprovedUsers(Room room)
        {
            client.RequestGetApprovedUsersInRoomAsync(room.GetGuid());
        }
    }
}