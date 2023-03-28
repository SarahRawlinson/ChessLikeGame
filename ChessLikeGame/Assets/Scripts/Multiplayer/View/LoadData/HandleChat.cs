using System;
using System.Collections.Generic;
using MessageServer.Data;
using Multiplayer.Controllers;
using Multiplayer.View.UI;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class HandleChat : MonoBehaviour
    {
        class Chat
        {
            public SendTextToWindow window;
            public User user;

            public Chat(SendTextToWindow textToWindow, User chattingWith)
            {
                user = chattingWith;
                window = textToWindow;
            }
        }

        [SerializeField] private Transform window;
        [SerializeField] private SendTextToWindow prefab;
        private Dictionary<User, Chat> chats = new Dictionary<User, Chat>();

        private void Start()
        {
            WebSocketConnection.onMessageRecieved += MessageRecievedFromSocket;
        }

        private void MessageRecievedFromSocket((User user, string message) obj)
        {
            if (!chats.ContainsKey(obj.user))
            {
                chats[obj.user].window.SendMessageToUI("", $"A new chat with {obj.user.GetUserName()} has started");
                StartNewChatWithUser(obj.user);
            }
            chats[obj.user].window.SendMessageToUI(obj.user.GetUserName(), obj.message);
        }

        public void StartNewChatWithUser(User user)
        {
            if (chats.ContainsKey(user))
            {
                chats[user].window.gameObject.SetActive(true);
                return;
            }
            GameObject obj =  Instantiate(prefab.gameObject, window);
            SendTextToWindow sendTextToWindow = obj.GetComponent<SendTextToWindow>();
            sendTextToWindow.SetChattingWith(user.GetUserName(), user);
            sendTextToWindow.onSendMessage += SendMessage;
            chats.Add(user, new Chat(sendTextToWindow, user));
        }

        private void SendMessage((User user, string message) obj)
        {
            FindObjectOfType<WebSocketConnection>().SendMessageToUser(obj.user, obj.message);
        }
    }
}