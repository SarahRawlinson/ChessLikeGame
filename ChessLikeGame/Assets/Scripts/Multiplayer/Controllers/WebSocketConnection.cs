using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
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
        public static event Action<string> onUserJoinedGame;
        public static event Action<string> onUserJoinedChat;
        public static event Action<string> onUserLeftGame;
        public static event Action<string> onUserLeftChat;
        public static event Action<List<Room>>onHostsList;
        public static event Action<List<Room>>onChatRoomList;
        public static event Action<int> onHostGame;
        public static event Action<int> onHostChat;
        public static event Action<string> onJoinedGame;
        public static event Action<string> onJoinedChat;
        public event Action<(int RoomID, string Message)> onChatRoomMessageRecieved;
        public event Action<(int RoomID, string Message)> onGameRoomMessageRecieved;
        
        
        
        public User user { get; set; }
        private int clientID = -1;
        private bool refresh;
        private void Start()
        {
            client.SetDisconnectOnFailAuthentication(true);
            client.onMessageRecievedEvent += MessageReceived;
            client.onAuthenticateEvent += Authentication;
            client.onUserListRecievedEvent += UserListReceived;
            client.onUserJoinedEvent += UserJoined;
            client.onUserLeftEvent += UserLeft;
            client.onRoomListRecievedEvent += RoomListReceived;
            client.onRoomCreatedEvent += CreatedRoom;
            client.onRoomJoinedEvent += JoinedRoom;
            client.onRoomMessageRecievedEvent += RoomMessageReceived;
            client.onIDRecievedEvent += ClientIDReceived;
            client.onIncomingWebSocketMessage += LogMessage;
        }

        private void LogMessage(string obj)
        {
            if (obj.Contains("RECIEVEMESSAGE")) Debug.LogWarning($"WEB SOCKET RECEIVED MESSAGE {obj}");
            else
            {
                Debug.Log($"WEB SOCKET RECEIVED MESSAGE {obj}");
            }
        }

        private void ClientIDReceived(int obj)
        {
            Debug.Log($"ClientIDReceived={obj}");
            clientID = obj;
        }

        private void RoomMessageReceived((int RoomID, string Message) obj)
        {
            Debug.Log($"RoomMessageReceived={obj}");
            //TODO: WORK OUT WHICH
            onChatRoomMessageRecieved?.Invoke(obj);
            onGameRoomMessageRecieved?.Invoke(obj);
        }

        private static void JoinedRoom(string obj)
        {
            Debug.Log($"JoinedRoom={obj}");
            //TODO: WORK OUT WHICH
            onJoinedGame?.Invoke(obj);
            onJoinedChat?.Invoke(obj);
        }

        private static void CreatedRoom(int obj)
        {
            Debug.Log($"CreatedRoom={obj}");
            //TODO: WORK OUT WHICH
            onHostGame?.Invoke(obj);
            onHostChat?.Invoke(obj);
        }

        private static void RoomListReceived(List<Room> obj)
        {
            // Debug.Log($"RoomListReceived={obj}");
            //TODO: WORK OUT WHICH
            onHostsList?.Invoke(obj);
            onChatRoomList?.Invoke(obj);
        }

        private void UserLeft(string obj)
        {
            Debug.Log($"UserLeft={obj}");
            //TODO: WORK OUT WHICH
            onUserLeftGame?.Invoke(obj);
            onUserLeftGame?.Invoke(obj);
        }

        private void UserJoined(string obj)
        {
            Debug.Log($"UserJoined={obj}");
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
        public async void Connect(User userData, string password)
        {
            Debug.Log("login");
            await client.Connect();
            StartCoroutine(nameof(StartConnection));
            await client.Authenticate(userData.GetUserName(), password);
        }

        public async Task StartConnection()
        {
            await client.Listen();
            Debug.Log("Stopped Listening");
        }

        public IEnumerator RefreshSubscribed()
        {
            if (refresh) yield break;
            refresh = true;
            while (refresh)
            {
                client.RequestRoomList();
                client.UpdateUserList();
                yield return new WaitForSeconds(5.0f);
            }
            Debug.Log("Stopped Asking Server For Updates");
        }
        
        public void SendMessageToUser(User user, string Message)
        {
            client.SendMessageToUser(user, Message);
        }

        
        public void GetRoomList()
        {
            Debug.Log("asked for rooms");
            client.RequestRoomList();
        }
        
        public void CreateNewRoom(string meta, int roomSize, bool isPublic)
        {
            client.CreateRoom(meta, roomSize, isPublic);
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
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Stop Web Socket", CancellationToken.None);
        }
        
        
    }
}