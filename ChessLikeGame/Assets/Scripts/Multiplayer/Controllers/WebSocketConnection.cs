using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using LibObjects;
using MessageServer.Data;
using NetClient;
using UnityEngine;


namespace Multiplayer.Controllers
{
    public class WebSocketConnection : MonoBehaviour
    {
        private ClientWebSocket webSocket;

        private Client client = new Client();

        public static event Action<(User user, string message)> onMessageRecieved;
        public static event Action<bool> onAuthenicate;
        public static event Action<List<MessageServer.Data.User>> onUsersList;
        public static event Action<(User user, Guid roomGuid)> onUserJoinedGame;
        public static event Action<(User user, Guid roomGuid)> onUserJoinedChat;
        public static event Action<(User user, Guid roomGuid)> onUserLeftGame;
        public static event Action<(User user, Guid roomGuid)> onUserLeftChat;
        public static event Action<List<Room>>onHostsList;
        public static event Action<List<Room>>onChatRoomList;
        public static event Action<Room> onHostGame;
        public static event Action<Room> onHostChat;
        public static event Action<Room> onJoinedGame;
        public static event Action<Room> onJoinedChat;
        public static event Action<(Room room, User user, string Message)> onChatRoomMessageRecieved;
        public static event Action<(Room room, User user, string Message)> onGameRoomMessageRecieved;
        [SerializeField] private bool refreshSubscribed = false;
        
        
        
        public User user { get; set; }
        private Guid clientID = Guid.Empty;
        private bool refresh;
        private void Start()
        {
            client.SetDisconnectOnFailAuthentication(true);
            client.onRecievedMessageFromUserEvent += MessageReceived;
            client.onReceivedAuthenticateEvent += Authentication;
            client.onRecievedUserListEvent += UserListReceived;
            client.onRecievedUserJoinedRoomEvent += UserJoinedRoom;
            client.onRecievedUserLeftRoomEvent += UserLeftRoom;
            client.onRecievedRoomListEvent += RoomListReceived;
            client.onRecievedRoomCreatedEvent += CreatedRoom;
            client.onRecievedRoomJoinedEvent += JoinedRoom;
            client.onRecievedRoomMessageEvent += RoomMessageReceived;
            client.onRecievedGuidEvent += ClientGuidReceived;
            client.onIncomingWebSocketMessageEvent += LogMessageReceived;
            client.onRecievedUserWithGuidEvent += UserWithGuidReceived;
            client.onMessageSentToSocketEvent += LogMessageSent;
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
            //TODO: WORK OUT WHICH
            onChatRoomMessageRecieved?.Invoke(obj);
            onGameRoomMessageRecieved?.Invoke(obj);
        }

        private void JoinedRoom(Room obj)
        {
            Debug.Log($"JoinedRoom={obj}");
            //TODO: WORK OUT WHICH
            onJoinedGame?.Invoke(obj);
            onJoinedChat?.Invoke(obj);
        }

        private void CreatedRoom(Room obj)
        {
            Debug.Log($"CreatedRoom={obj}");
            //TODO: WORK OUT WHICH
            onHostGame?.Invoke(obj);
            onHostChat?.Invoke(obj);
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
            // Debug.Log($"RoomListReceived={obj}");
            //TODO: WORK OUT WHICH
            onHostsList?.Invoke(obj);
            onChatRoomList?.Invoke(obj);
        }

        private void UserLeftRoom((User user, Guid roomGuid) obj)
        {
            Debug.Log($"UserLeftRoom={obj}");
            //TODO: WORK OUT WHICH
            onUserLeftGame?.Invoke(obj);
            onUserLeftGame?.Invoke(obj);
        }

        private void UserJoinedRoom((User user, Guid id) obj)
        {
            Debug.Log($"UserJoinedRoom={obj.user.GetUserName()}");
            //TODO: WORK OUT WHICH
            onUserJoinedChat?.Invoke(obj);
            onUserJoinedGame?.Invoke(obj);
        }


        private static void UserListReceived(List<MessageServer.Data.User> obj)
        {
            // Debug.Log($"UserListReceived={obj}");
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
        public async void Connect(string userName, string password)
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
                client.RequestRoomList();
                client.RequestUserList();
                yield return new WaitForSeconds(5.0f);
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
        
        public void CreateNewGameRoom(int roomSize, bool isPublic)
        {
            client.RequestCreateRoom("ChessGameRoom", roomSize, isPublic);
        }
        
        public void CreateNewChatRoom(int roomSize, bool isPublic)
        {
            client.RequestCreateRoom("ChessChatRoom", roomSize, isPublic);
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
            // await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Stop Web Socket", CancellationToken.None);
        }


        public void SendMessageToRoom(Room room, string objMessage)
        {
            client.RequestSendMessageToRoomAsync(room.GetGuid(), objMessage);
        }

        public void JoinRoom(Guid room)
        {
            client.RequestToAddUserToRoom(user, room);
        }
    }
}