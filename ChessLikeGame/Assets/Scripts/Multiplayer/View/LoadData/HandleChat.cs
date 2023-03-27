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
            FindObjectOfType<WebSocketConnection>().SendMessageToUser(obj.user.GetUserName(), obj.message);
        }
    }
}