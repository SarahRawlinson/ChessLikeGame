using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using LibObjects;
using MessageServer.Data;
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
        public static event Action<List<User>> onUsersList;
        public static event Action<(User user, Guid roomGuid)> onUserJoinedRoom;
        public static event Action<(User user, Guid roomGuid)> onUserLeftRoom;
        public static event Action<List<Room>>onHostsList;
        public static event Action<List<Room>>onChatRoomList;
        public static event Action<Room> onHostGame;
        public static event Action<Room> onHostChat;
        public static event Action<Room> onRecievedYouWhereRemovedFromTheRoom;
        public static event Action<Room> onRecievedYouHaveBeenBannedFromRoom;
        public static event Action<Room> onRecievedYouAreNoLongerBannedFromRoom;
        public static event Action<Room> onRecievedYouHaveBeenApprovedForRoom;
        public static event Action<Room> onRecievedYouAreNoLongerApprovedForRoom;
        public static event Action<Room> onRecievedRoomLeft;
        public static event Action<Room> onJoinedChat;
        public static event Action<Room> onJoinedGame;
        public static event Action<(Room room, User user, string Message)> onChatRoomMessageRecieved;
        public static event Action<(Room room, User user, string Message)> onGameRoomMessageRecieved;
        public static event Action<(User user, string messageSent)> onReceivedCommunicationToAll;
        public static event Action<Room> onRoomDestroyed;
        public static event Action<(Room room, List<User> users)> onReceivedUsersListInRoom;
        public static event Action<(Room room, List<User> users)> onReceivedApprovedUsersListInRoom;
        public static event Action<(Room room, List<User> users)> onReceivedBannedUsersListInRoom;
        public static event Action<User> onSetUser;

        [SerializeField] private bool refreshSubscribed = false;
        [SerializeField] private float refreshTime = 10f;
        public User user { get; set; }
        private Guid clientID = Guid.Empty;
        private bool refresh;
        private ClientWebSocket webSocket;
        private readonly Client client = new Client();
        
        private void Start()
        {
            client.SetDisconnectOnFailAuthentication(true);
            client.onRecievedMessageFromUserEvent += MessageReceived;
            client.onReceivedAuthenticateEvent += Authentication;
            client.onRecievedRoomDestroyedEvent += RoomDestroyed;
            client.onRecievedUserListEvent += UserListReceived;
            client.onRecievedUserJoinedRoomEvent += UserJoinedRoom;
            client.onRecievedUserLeftRoomEvent += UserLeftRoom;
            client.onRecievedRoomListEvent += RoomListReceived;
            client.onRecievedRoomCreatedEvent += CreatedRoom;
            client.onRecievedRemovedFromTheRoomEvent += RemovedFromTheRoom;
            client.onRecievedBannedFromRoomEvent += BannedFromTheRoom;
            client.onRecievedNoLongerBannedFromRoomEvent += NoLongerBannedFromRoom;
            client.onRecievedApprovedForRoomEvent += ApprovedForRoom;
            client.onRecievedNoLongerApprovedForRoomEvent += NoLongerApprovedForRoom;
            client.onRecievedRoomJoinedEvent += JoinedRoom;
            client.onRecievedRoomLeftEvent += RoomLeft;
            client.onRecievedRoomMessageEvent += RoomMessageReceived;
            client.onReceivedUsersListInRoomEvent += UsersInRoom;
            client.onReceivedMessageWasReceivedByUserEvent += ReceivedMessageWasReceivedByUser;
            client.onReceivedCommunicationToAllEvent += ReceivedCommunicationToAll;
            client.onReceivedErrorResponseFromServerEvent += ReceivedErrorResponseFromServer;
            client.onRecievedGuidEvent += ClientGuidReceived;
            client.onIncomingWebSocketMessageEvent += LogMessageReceived;
            client.onRecievedUserWithGuidEvent += UserWithGuidReceived;
            client.onMessageSentToSocketEvent += LogMessageSent;
            client.onReceivedApprovedUsersListInRoomEvent += ApprovedUsersInRoom;
            client.onReceivedBannedUsersListInRoomEvent += BannedUsersInRoom;
        }

        private void BannedUsersInRoom((Room room, List<User> users) obj)
        {
            onReceivedBannedUsersListInRoom?.Invoke(obj);
        }

        private void ApprovedUsersInRoom((Room room, List<User> users) obj)
        {
            onReceivedApprovedUsersListInRoom?.Invoke(obj);
        }


        private void RoomLeft(Room obj)
        {
            Debug.Log($"RoomLeft={obj.GetRoomName()}");
            onRecievedRoomLeft?.Invoke(obj);
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
            Debug.Log($"RoomMessageReceived={obj.user.GetUserName()} {obj.room.GetRoomName()} {obj.Message}");
            if (obj.room.GetMeta() == ChatRoomMeta)
            {
                onChatRoomMessageRecieved?.Invoke(obj);
            }
            if (obj.room.GetMeta() == GameRoomMeta)
            {
                onGameRoomMessageRecieved?.Invoke(obj);
            }
        }

        private void JoinedRoom(Room obj)
        {
            Debug.Log($"JoinedRoom={obj.GetRoomName()}");
            if (obj.GetMeta() == ChatRoomMeta)
            {
                onJoinedChat?.Invoke(obj);
            }
            if (obj.GetMeta() == GameRoomMeta)
            {
                onJoinedGame?.Invoke(obj);
            }
        }

        private void CreatedRoom(Room obj)
        {
            Debug.Log($"CreatedRoom={obj.GetRoomName()}");
            if (obj.GetMeta() == ChatRoomMeta)
            {
                onHostChat?.Invoke(obj);
            }
            if (obj.GetMeta() == GameRoomMeta)
            {
                onHostGame?.Invoke(obj);
            }
        }

        private void LogMessageReceived(string obj)
        {
            if (obj.Contains("RECIEVEMESSAGE")) Debug.LogWarning($"WEB SOCKET RECEIVED MESSAGE {obj}");
            else
            {
                Debug.Log($"WEB SOCKET RECEIVED MESSAGE {obj}");
            }
        }
        
        private void LogMessageSent(string obj)
        {
            if (obj.Contains("RECIEVEMESSAGE")) Debug.LogWarning($"WEB SOCKET SENT MESSAGE {obj}");
            else
            {
                Debug.Log($"WEB SOCKET SENT MESSAGE {obj}");
            }
        }

        private void ClientGuidReceived(Guid obj)
        {
            Debug.Log($"ClientGuidReceived={obj}");
            clientID = obj;
        }

        private static void RoomListReceived(List<Room> obj)
        {
            Debug.Log($"RoomListReceived={obj.Count}");
            onHostsList?.Invoke(obj);
            onChatRoomList?.Invoke(obj);
        }

        private void UserLeftRoom((User user, Guid roomGuid) obj)
        {
            Debug.Log($"UserLeftRoom={obj}");
            //TODO: WORK OUT WHICH
            onUserLeftRoom?.Invoke(obj);
        }

        private void UserJoinedRoom((User user, Guid id) obj)
        {
            Debug.Log($"UserJoinedRoom={obj.user.GetUserName()}");
            //TODO: WORK OUT WHICH
            onUserJoinedRoom?.Invoke(obj);
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
            await client.Connect();
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
                var uis = FindObjectsOfType<DisplayChatRoomUI>();
                for (var index = 0; index < uis.Length; index++)
                {
                    var display = uis[index];
                    display.AskForUsersOfRoom(this);
                    yield return new WaitForSeconds(1f);
                }

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