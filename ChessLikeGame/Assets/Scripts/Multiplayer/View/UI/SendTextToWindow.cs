using System;
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
        private User _user;
        public event Action<(User user, string message)> onSendMessage;

        public void SetChattingWith(string with, User user)
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
    }
}
