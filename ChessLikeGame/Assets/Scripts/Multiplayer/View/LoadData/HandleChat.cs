using System;
using System.Collections.Generic;
using LibObjects;
using MessageServer.Data;
using Multiplayer.Controllers;
using Multiplayer.View.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class HandleChat : MonoBehaviour
    {
        
        class Chat
        {
            public SendTextToWindow window;
            public SendTextToWindow.WindowType windowType;

            public Chat(SendTextToWindow textToWindow, User chattingWith)
            {
                windowType = new SendTextToWindow.WindowType(chattingWith);
                window = textToWindow;
            }
            public Chat(SendTextToWindow textToWindow, Room chattingWith)
            {
                windowType = new SendTextToWindow.WindowType(chattingWith);
                window = textToWindow;
            }
        }

        [SerializeField] private Transform window;
        [SerializeField] private SendTextToWindow prefab;
        private Dictionary<Guid, Chat> chatsWithUsers = new Dictionary<Guid, Chat>();
        private Dictionary<Guid, Chat> chatsWithRooms = new Dictionary<Guid, Chat>();
        private string userName = "Me";

        private void Start()
        {
            WebSocketConnection.onMessageRecieved += MessageReceivedFromSocket;
            WebSocketConnection.onChatRoomMessageRecieved += RoomMessageReceivedFromSocket;
            WebSocketConnection.onUserLeftRoom += UserLeftRoom;
            WebSocketConnection.onUserJoinedRoom += UserJoinedRoom;
            WebSocketConnection.onJoinedChat += ChatJoined;
            WebSocketConnection.onRecievedRoomLeft += RoomLeft;
            WebSocketConnection.onRoomDestroyed += RoomDestroyed;
            WebSocketConnection.onRecievedYouWhereRemovedFromTheRoom += RemovedFromRoom;
            WebSocketConnection.onSetUser += SetUser;
        }

        private void SetUser(User obj)
        {
            userName = obj.GetUserName();
        }

        public void OpenWindow(Room room)
        {
            if (chatsWithRooms.ContainsKey(room.GetGuid()))
            {
                chatsWithRooms[room.GetGuid()].window.Show();

            }
        }

        private void RoomLeft(Room obj)
        {
            DestroyWindow(obj);
        }

        private void DestroyWindow(Room obj)
        {
            if (chatsWithRooms.ContainsKey(obj.GetGuid()))
            {
                Destroy(chatsWithRooms[obj.GetGuid()].window.gameObject);
                chatsWithRooms.Remove(obj.GetGuid());
            }
        }

        private void RemovedFromRoom(Room obj)
        {
            DestroyWindow(obj);
        }

        private void RoomDestroyed(Room obj)
        {
            DestroyWindow(obj);
        }


        private void UserLeftRoom((User user, Guid roomGuid) obj)
        {
            chatsWithRooms[obj.roomGuid].window.SendMessageToUI("", $"{obj.user.GetUserName()} Has Left the Room");
        }
        
        private void UserJoinedRoom((User user, Guid roomGuid) obj)
        {
            chatsWithRooms[obj.roomGuid].window.SendMessageToUI("", $"{obj.user.GetUserName()} Has Joined the Room");
        }

        private void RoomMessageReceivedFromSocket((Room room, User user, string Message) obj)
        {
            if (!chatsWithRooms.ContainsKey(obj.room.GetGuid()))
            {
                chatsWithRooms[obj.room.GetGuid()].window.SendMessageToUI("", $"A new chat with {obj.user.GetUserName()} has started");
                StartNewChatWithRoom(obj.room);
            }
            chatsWithRooms[obj.room.GetGuid()].window.SendMessageToUI(obj.user.GetUserName(), obj.Message);
        }

        private void MessageReceivedFromSocket((User user, string message) obj)
        {
            if (!chatsWithUsers.ContainsKey(obj.user.GetUserGuid()))
            {
                chatsWithUsers[obj.user.GetUserGuid()].window.SendMessageToUI("", $"A new chat with {obj.user.GetUserName()} has started");
                StartNewChatWithUser(obj.user);
            }
            chatsWithUsers[obj.user.GetUserGuid()].window.SendMessageToUI(obj.user.GetUserName(), obj.message);
        }

        public void StartNewChatWithUser(User user)
        {
            if (chatsWithUsers.ContainsKey(user.GetUserGuid()))
            {
                chatsWithUsers[user.GetUserGuid()].window.gameObject.SetActive(true);
                return;
            }
            GameObject obj =  Instantiate(prefab.gameObject, window);
            SendTextToWindow sendTextToWindow = obj.GetComponent<SendTextToWindow>();
            var value = new Chat(sendTextToWindow, user);
            sendTextToWindow.SetChattingWith(user.GetUserName(), value.windowType, userName);
            sendTextToWindow.onSendMessage += SendMessage;
            
            chatsWithUsers.Add(user.GetUserGuid(), value);
        }
        
        public void StartNewChatWithRoom(Room room)
        {
            if (chatsWithRooms.ContainsKey(room.GetGuid()))
            {
                chatsWithRooms[room.GetGuid()].window.gameObject.SetActive(true);
                return;
            }
            FindObjectOfType<WebSocketConnection>().JoinRoom(room.GetGuid());
        }
        
        private void ChatJoined(Room obj)
        {
            GameObject objPrefab =  Instantiate(prefab.gameObject, window);
            SendTextToWindow sendTextToWindow = objPrefab.GetComponent<SendTextToWindow>();
            var value = new Chat(sendTextToWindow, obj);
            sendTextToWindow.SetChattingWith(obj.GetRoomName(), value.windowType, userName);
            sendTextToWindow.onSendMessage += SendMessage;
            chatsWithRooms.Add(obj.GetGuid(), value);
        }
        
        

        private void SendMessage((SendTextToWindow.WindowType windowType, string message) obj)
        {
            if (obj.windowType.IsUser)
            {
                FindObjectOfType<WebSocketConnection>().SendMessageToUser(obj.windowType.User, obj.message);
            }
            else
            {
                FindObjectOfType<WebSocketConnection>().SendMessageToRoom(obj.windowType.Room, obj.message);
            }
        }

        public void LeaveChat(Room room)
        {
            FindObjectOfType<WebSocketConnection>().LeaveRoom(room);
        }
    }
}