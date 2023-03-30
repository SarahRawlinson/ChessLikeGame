using System;
using LibObjects;
using MessageServer.Data;
using Multiplayer.View.DisplayData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.View.UI
{
    public class SendTextToWindow : MonoBehaviour
    {
        
        [SerializeField] private TMP_InputField _inputField;
        [FormerlySerializedAs("_chatMessageUIPrefab")] [SerializeField] private DisplayChatMessageUI displayChatMessageUIPrefab;
        [SerializeField] private ScrollContentUI _scrollContentUI;
        [SerializeField] private TMP_Text header;
        private WindowType _user;

        public event Action<(WindowType user, string message)> onSendMessage;

        public void SetChattingWith(string with, WindowType user)
        {
            _user = user;
            header.text = $"Chatting with {with}";
        }
        public void SendMessageToUI(string user, string message)
        {
            GameObject o = _scrollContentUI.AddContent(displayChatMessageUIPrefab.gameObject);
            DisplayChatMessageUI messageUI = o.GetComponent<DisplayChatMessageUI>();
            messageUI.SetMessage(user, message);
        }

        public void SendMessage()
        {
            onSendMessage?.Invoke((_user, _inputField.text));
            SendMessageToUI("Me",_inputField.text);
            _inputField.text = "";
        }
        
        public class WindowType
        {
            private bool isUser = false;
            private Room _room = null;

            public bool IsUser => isUser;

            public Room Room => _room;

            public User User => _user;

            private User _user = null;
            public WindowType(User user)
            {
                _user = user;
                isUser = true;
            }
            public WindowType(Room room)
            {
                _room = room;
                isUser = false;
            }
        }
    }
}
