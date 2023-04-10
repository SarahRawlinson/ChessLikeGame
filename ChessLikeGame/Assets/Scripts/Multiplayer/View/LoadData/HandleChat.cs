using System;
using System.Collections.Generic;
using LibObjects;
using Multiplayer.Controllers;
using Multiplayer.View.DisplayData;
using Multiplayer.View.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class HandleChat : MonoBehaviour
    {
        
        class Chat
        {
            public DisplayChatWindowUI windowUI;
            public DisplayChatWindowUI.WindowType windowType;
            public List<User> usersInRoom = new List<User>();

            public Chat(DisplayChatWindowUI textToWindowUI, User chattingWith)
            {
                windowType = new DisplayChatWindowUI.WindowType(chattingWith);
                windowUI = textToWindowUI;
            }
            public Chat(DisplayChatWindowUI textToWindowUI, Room chattingWith)
            {
                windowType = new DisplayChatWindowUI.WindowType(chattingWith);
                windowUI = textToWindowUI;
            }
        }

        [SerializeField] private Transform window;
        [SerializeField] private DisplayChatWindowUI prefab;
        [SerializeField] private LoadChatUsersUI chatUsersList;
        private Dictionary<Guid, Chat> chatsWithUsers = new Dictionary<Guid, Chat>();
        private Dictionary<Guid, Chat> chatsWithRooms = new Dictionary<Guid, Chat>();
        private string userName = "Me";

        private void Start()
        {
            chatUsersList.onCreatedUserUI += UserUICreated;
            chatUsersList.onDestroyedUserUI += UserUIDestroyed;
            WebSocketConnection.onMessageRecieved += MessageReceivedFromSocket;
            WebSocketConnection.onChatRoomMessageRecieved += RoomMessageReceivedFromSocket;
            WebSocketConnection.onChatRoomPrivateMessageRecieved += PrivateRoomMessageReceivedFromSocket;
            WebSocketConnection.onUserLeftChatRoom += UserLeftRoom;
            WebSocketConnection.onUserJoinedChatRoom += UserJoinedRoom;
            WebSocketConnection.onJoinedChat += ChatJoined;
            WebSocketConnection.onLeftChat += RoomLeft;
            WebSocketConnection.onRoomDestroyed += RoomDestroyed;
            WebSocketConnection.onRecievedYouWhereRemovedFromTheRoom += RemovedFromRoom;
            WebSocketConnection.onSetUser += SetUser;
            WebSocketConnection.onLoggedOff += DestroyWindows;
            WebSocketConnection.onReceivedUsersListInRoom += AddUsersToRoom;
        }

        private void AddUsersToRoom((Room room, List<User> users) obj)
        {
            if (chatsWithRooms.ContainsKey(obj.room.GetGuid()))
            {
                chatsWithRooms[obj.room.GetGuid()].usersInRoom = obj.users;
            }
        }

        private void DestroyWindows()
        {
            foreach (var roomWindow in chatsWithRooms)
            {
                Destroy(roomWindow.Value.windowUI.gameObject);
            }
            chatsWithRooms.Clear();
            foreach (var roomWindow in chatsWithUsers)
            {
                Destroy(roomWindow.Value.windowUI.gameObject);
            }
            chatsWithUsers.Clear();
        }

        private void OnDestroy()
        {
            chatUsersList.onCreatedUserUI -= UserUICreated;
            chatUsersList.onDestroyedUserUI -= UserUIDestroyed;
            WebSocketConnection.onMessageRecieved -= MessageReceivedFromSocket;
            WebSocketConnection.onChatRoomMessageRecieved -= RoomMessageReceivedFromSocket;
            WebSocketConnection.onUserLeftChatRoom -= UserLeftRoom;
            WebSocketConnection.onUserJoinedChatRoom -= UserJoinedRoom;
            WebSocketConnection.onJoinedChat -= ChatJoined;
            WebSocketConnection.onLeftChat -= RoomLeft;
            WebSocketConnection.onRoomDestroyed -= RoomDestroyed;
            WebSocketConnection.onRecievedYouWhereRemovedFromTheRoom -= RemovedFromRoom;
            WebSocketConnection.onSetUser -= SetUser;
            WebSocketConnection.onLoggedOff -= DestroyWindows;
        }
        private void UserUIDestroyed(DisplayChatUserUI obj)
        {
            obj.onSelectedUser -= ChatChat;
            LeaveChat(obj.GetUser());
        }

        private void UserUICreated(DisplayChatUserUI obj)
        {
            obj.onSelectedUser += ChatChat;
        }

        private void LeaveChat(User obj)
        {
            DestroyWindow(obj);
        }

        private void SetUser(User obj)
        {
            userName = obj.GetUserName();
        }

        public void OpenWindow(Room room)
        {
            HideAllChats();
            if (chatsWithRooms.ContainsKey(room.GetGuid()))
            {
                chatsWithRooms[room.GetGuid()].windowUI.Show();
            }
        }
        
        public void OpenWindow(User user)
        {
            HideAllChats();
            if (chatsWithUsers.ContainsKey(user.GetUserGuid()))
            {
                chatsWithUsers[user.GetUserGuid()].windowUI.Show();
            }
        }

        private void HideAllChats()
        {
            foreach (var userKVP in chatsWithUsers)
            {
                chatsWithUsers[userKVP.Key].windowUI.Hide();
            }

            foreach (var roomKVP in chatsWithRooms)
            {
                chatsWithRooms[roomKVP.Key].windowUI.Hide();
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
                Destroy(chatsWithRooms[obj.GetGuid()].windowUI.gameObject);
                chatsWithRooms.Remove(obj.GetGuid());
            }
        }
        
        private void DestroyWindow(User obj)
        {
            if (chatsWithUsers.ContainsKey(obj.GetUserGuid()))
            {
                Destroy(chatsWithUsers[obj.GetUserGuid()].windowUI.gameObject);
                chatsWithUsers.Remove(obj.GetUserGuid());
            }
        }

        private void RemovedFromRoom(Room obj)
        {
            DestroyWindow(obj);
        }

        private void RoomDestroyed(Room obj)
        {
            DestroyWindow(obj);
            FindObjectOfType<WebSocketConnection>().RefreshRooms();
        }


        private void UserLeftRoom((User user, Guid roomGuid) obj)
        {
            chatsWithRooms[obj.roomGuid].windowUI.SendMessageToUI("", $"{obj.user.GetUserName()} Has Left the Room");
        }
        
        private void UserJoinedRoom((User user, Guid roomGuid) obj)
        {
            chatsWithRooms[obj.roomGuid].windowUI.SendMessageToUI("", $"{obj.user.GetUserName()} Has Joined the Room");
        }

        private void RoomMessageReceivedFromSocket((Room room, User user, string Message) obj)
        {
            if (!chatsWithRooms.ContainsKey(obj.room.GetGuid()))
            {
                chatsWithRooms[obj.room.GetGuid()].windowUI.SendMessageToUI("", $"A new chat with {obj.user.GetUserName()} has started");
                StartNewChatWithRoom(obj.room);
            }
            chatsWithRooms[obj.room.GetGuid()].windowUI.SendMessageToUI(obj.user.GetUserName(), obj.Message);
        }
        private void PrivateRoomMessageReceivedFromSocket((Room room, User user, string Message) obj)
        {
            if (!chatsWithRooms.ContainsKey(obj.room.GetGuid()))
            {
                chatsWithRooms[obj.room.GetGuid()].windowUI.SendMessageToUI("", $"A new chat with {obj.user.GetUserName()} has started");
                StartNewChatWithRoom(obj.room);
            }
            chatsWithRooms[obj.room.GetGuid()].windowUI.SendMessageToUI(obj.user.GetUserName(), obj.Message, true);
        }

        private void MessageReceivedFromSocket((User user, string message) obj)
        {
            if (!chatsWithUsers.ContainsKey(obj.user.GetUserGuid()))
            {
                chatsWithUsers[obj.user.GetUserGuid()].windowUI.SendMessageToUI("", $"A new chat with {obj.user.GetUserName()} has started");
                StartNewChatWithUser(obj.user);
            }
            chatsWithUsers[obj.user.GetUserGuid()].windowUI.SendMessageToUI(obj.user.GetUserName(), obj.message, false);
        }

        public void StartNewChatWithUser(User user)
        {
            if (chatsWithUsers.ContainsKey(user.GetUserGuid()))
            {
                OpenWindow(user);
                return;
            }
            GameObject obj =  Instantiate(prefab.gameObject, window);
            DisplayChatWindowUI displayChatWindowUI = obj.GetComponent<DisplayChatWindowUI>();
            var value = new Chat(displayChatWindowUI, user);
            displayChatWindowUI.SetChattingWith(user.GetUserName(), value.windowType, userName);
            displayChatWindowUI.onSendMessage += SendMessage;
            
            chatsWithUsers.Add(user.GetUserGuid(), value);
        }
        
        public void StartNewChatWithRoom(Room room)
        {
            if (chatsWithRooms.ContainsKey(room.GetGuid()))
            {
                OpenWindow(room);
                return;
            }
            FindObjectOfType<WebSocketConnection>().JoinRoom(room.GetGuid());
        }
        
        private void ChatJoined(Room obj)
        {
            GameObject objPrefab =  Instantiate(prefab.gameObject, window);
            DisplayChatWindowUI displayChatWindowUI = objPrefab.GetComponent<DisplayChatWindowUI>();
            var value = new Chat(displayChatWindowUI, obj);
            displayChatWindowUI.SetChattingWith(obj.GetRoomName(), value.windowType, userName);
            displayChatWindowUI.onSendMessage += SendMessage;
            chatsWithRooms.Add(obj.GetGuid(), value);
        }
        
        

        private void SendMessage((DisplayChatWindowUI.WindowType windowType, string message) obj)
        {
            if (obj.windowType.IsUser)
            {
                FindObjectOfType<WebSocketConnection>().SendMessageToUser(obj.windowType.User, obj.message);
            }
            else
            {
                if (SendPrivateMessage(obj)) return;
                FindObjectOfType<WebSocketConnection>().SendMessageToRoom(obj.windowType.Room, obj.message);
            }
        }

        private bool SendPrivateMessage((DisplayChatWindowUI.WindowType windowType, string message) obj)
        {
            if (obj.message.Substring(0, 3).ToLower() == "pm@")
            {
                var indexOf = obj.message.IndexOf(" ");
                if (indexOf > -1)
                {
                    string firstWord = obj.message.Substring(0, indexOf);
                    if (firstWord.Length > 3)
                    {
                        firstWord = firstWord.Substring(3).ToLower();
                        Debug.Log(firstWord);
                        foreach (var user in chatsWithRooms[obj.windowType.Room.GetGuid()].usersInRoom)
                        {
                            if (firstWord == user.GetUserName().ToLower())
                            {
                                FindObjectOfType<WebSocketConnection>()
                                    .SendMessageToUserInRoom(obj.windowType.Room, obj.message.Substring(4 + firstWord.Length),
                                        user);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public void LeaveChat(Room room)
        {
            FindObjectOfType<WebSocketConnection>().LeaveRoom(room);
        }

        private void ChatChat(User obj)
        {
            StartNewChatWithUser(obj);
        }
    }
}