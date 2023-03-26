using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using MessageServer.Data;
using NetClient;
using UnityEngine;
using User = Multiplayer.Models.Connection.User;


namespace Multiplayer.Controllers
{
    public class WebSocketConnection : MonoBehaviour
    {
        private ClientWebSocket webSocket;

        private Client client = new Client();

        public static event Action<string> onMessageRecieved;
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

        private void Start()
        {
            client.onMessageRecievedEvent += MessageRecieved;
            client.onAuthenticateEvent += Authentication;
            client.onUserListRecievedEvent += UserListReceived;
            client.onUserJoinedEvent += UserJoined;
            client.onUserLeftEvent += UserLeft;
            client.onRoomListRecievedEvent += RoomListReceived;
            client.onRoomCreatedEvent += CreatedRoom;
            client.onRoomJoinedEvent += JoinedRoom;
            client.onRoomMessageRecievedEvent += RoomMessageRecieved;
            client.onIDRecievedEvent += ClientIDRecieved;
        }

        private void ClientIDRecieved(int obj)
        {
            clientID = obj;
        }

        private void RoomMessageRecieved((int RoomID, string Message) obj)
        {
            //TODO: WORK OUT WHICH
            onChatRoomMessageRecieved?.Invoke(obj);
            onGameRoomMessageRecieved?.Invoke(obj);
        }

        private void JoinedRoom(string obj)
        {
            //TODO: WORK OUT WHICH
            onJoinedGame?.Invoke(obj);
            onJoinedChat?.Invoke(obj);
        }

        private void CreatedRoom(int obj)
        {
            //TODO: WORK OUT WHICH
            onHostGame?.Invoke(obj);
            onHostChat?.Invoke(obj);
        }

        private void RoomListReceived(List<Room> obj)
        {
            //TODO: WORK OUT WHICH
            onHostsList?.Invoke(obj);
            onChatRoomList?.Invoke(obj);
        }

        private void UserLeft(string obj)
        {
            //TODO: WORK OUT WHICH
            onUserLeftGame?.Invoke(obj);
            onUserLeftGame?.Invoke(obj);
        }

        private void UserJoined(string obj)
        {
            //TODO: WORK OUT WHICH
            onUserJoinedChat?.Invoke(obj);
            onUserJoinedGame?.Invoke(obj);
        }


        private void UserListReceived(List<MessageServer.Data.User> obj)
        {
            onUsersList?.Invoke(obj);
        }

        private void MessageRecieved(string obj)
        {
            onMessageRecieved?.Invoke(obj);
        }

        private void Authentication(bool obj)
        {
            onAuthenicate?.Invoke(obj);
        }

        // Start is called before the first frame update
        public async void Connect(User userData)
        {
            Debug.Log("login");
            await Task.FromResult(client.Connect());
            await Task.FromResult(client.Authenticate(userData.Username, userData.Password));
        }

        
        public void SendMessageToUser(string userName, string Message)
        {
            client.SendMessageToUser(userName, Message);
        }

        
        public void GetRoomList()
        {
            client.GetRoomList();
        }
        
        public void CreateNewRoom(string meta, int roomSize, bool isPublic)
        {
            client.CreateRoom(meta, roomSize, isPublic);
        }


        // Close the WebSocket connection when the script is destroyed
        private async void OnDestroy()
        {
            await client.CloseSocket();
        }
        
    }
}