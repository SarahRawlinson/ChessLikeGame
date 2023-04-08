using System;
using System.Collections.Generic;
using LibObjects;
using Multiplayer.Controllers;
using Multiplayer.View.LoadData;
using Multiplayer.View.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.View.DisplayData
{
    public class DisplayChatWindowUI : MonoBehaviour
    {
        [SerializeField] private GameObject ActivateOnOpen;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private DisplayChatMessageUI displayChatMessageUIPrefab;
        [SerializeField] private ScrollContentUI _scrollContentUI;
        [SerializeField] private TMP_Text header;
        [SerializeField] private ToggleButton toggleMenuButton;
        [SerializeField] private GameObject menuGameObject;
        [SerializeField] private GameObject roomInfoGameObject;
        [SerializeField] private ToggleButton toggleRoomInfoButton;
        [SerializeField] private DisplayChatRoomInfoUI displayChatRoomInfoUI;
        [SerializeField] private DisplayUserMenuUI displayUserMenuUI;
        private WindowType windowInfo;
        private string _userName = "Me";

        public event Action<(WindowType user, string message)> onSendMessage;

        public void SetChattingWith(string with, WindowType window, string userName)
        {
            windowInfo = window;
            header.text = $"Chatting with {with}";
            _userName = userName;
            if (!window.IsUser)
            {
                displayChatRoomInfoUI.SetData(window.Room);
                displayUserMenuUI.SetRoom(window.Room);
            }
        }
        public void SendMessageToUI(string user, string message)
        {
            GameObject o = _scrollContentUI.AddContent(displayChatMessageUIPrefab.gameObject);
            DisplayChatMessageUI messageUI = o.GetComponent<DisplayChatMessageUI>();
            messageUI.SetMessage(user, message);
        }

        public void SendMessage()
        {
            onSendMessage?.Invoke((windowInfo, _inputField.text));
            if (windowInfo.IsUser)
            {
                SendMessageToUI(_userName,_inputField.text);
            }
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

        public void Show()
        {
            ActivateOnOpen.SetActive(true);
        }
        
        public void Hide()
        {
            ActivateOnOpen.SetActive(false);
        }

        private void Start()
        {
            menuGameObject.SetActive(toggleMenuButton.IsOn());
            roomInfoGameObject.SetActive(toggleRoomInfoButton.IsOn());
        }

        public void ToggleUsersMenu()
        {
            if (windowInfo.IsUser) return;
            toggleMenuButton.Toggle();
            menuGameObject.SetActive(toggleMenuButton.IsOn());
        }
        
        public void ToggleRoomInfo()
        {
            if (windowInfo.IsUser) return;
            toggleRoomInfoButton.Toggle();
            roomInfoGameObject.SetActive(toggleRoomInfoButton.IsOn());
            if (toggleRoomInfoButton.IsOn())
            {
                displayChatRoomInfoUI.SetData(windowInfo.Room);
            }
        }
        
        public void LeaveRoom()
        {
            if (windowInfo.IsUser) return;
            FindObjectOfType<WebSocketConnection>().LeaveRoom(windowInfo.Room);
        }
        
    }
}
