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
        }

        private void RoomLeft(Room obj)
        {
            onRecievedRoomLeft?.Invoke(obj);
        }

        private void NoLongerApprovedForRoom(Room obj)
        {
            onRecievedYouAreNoLongerApprovedForRoom?.Invoke(obj);
        }

        private void ApprovedForRoom(Room obj)
        {
            onRecievedYouHaveBeenApprovedForRoom?.Invoke(obj);
        }

        private void NoLongerBannedFromRoom(Room obj)
        {
            onRecievedYouAreNoLongerBannedFromRoom?.Invoke(obj);
        }

        private void BannedFromTheRoom(Room obj)
        {
            onRecievedYouHaveBeenBannedFromRoom?.Invoke(obj);
        }

        private void RemovedFromTheRoom(Room obj)
        {
            onRecievedYouWhereRemovedFromTheRoom?.Invoke(obj);
        }

        public User GetClientUser()
        {
            return user;
        }

        private void ReceivedErrorResponseFromServer((CommunicationTypeEnum comEnum, string message) obj)
        {
            Debug.Log($"{obj.comEnum.ToString()}");
        }

        private void ReceivedMessageWasReceivedByUser((User user, string messageSent) obj)
        {
            Debug.Log($"{obj.user.GetUserName()} received message: {obj.messageSent}");
        }

        private void ReceivedCommunicationToAll((User user, string messageSent) obj)
        {
            onReceivedCommunicationToAll?.Invoke(obj);
        }

        private void UsersInRoom((Room room, List<User> users) obj)
        {
            throw new NotImplementedException();
        }

        private void RoomDestroyed(Room obj)
        {
            onRoomDestroyed?.Invoke(obj);
        }

        private void UserWithGuidReceived((User user, Guid guid) obj)
        {
            if (obj.guid == clientID)
            {
                user = obj.user;
            }
        }

        private void RoomMessageReceived((Room room, User user, string Message) obj)
        {
            Debug.Log($"RoomMessageReceived={obj}");
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
            Debug.Log($"JoinedRoom={obj}");
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
            Debug.Log($"CreatedRoom={obj}");
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
                StopCoroutine(nameof(CloseSocket));
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
            StartCoroutine(nameof(StartConnection));
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
                foreach (var display in FindObjectsOfType<DisplayChatRoomUI>())
                {
                    display.AskForUsersOfRoom(this);
                }
                yield return new WaitForSeconds(5f);
                client.RequestRoomList();
                client.RequestUserList();
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
            client.RequestGetUsersInRoomAsync(room.GetGuid());
        }

        public void LeaveRoom(Room room)
        {
            client.RequestRemoveUserFromRoom(room, user);
        }
    }
}