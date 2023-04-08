using System;
using System.Collections.Generic;
using LibObjects;
using MessageServer.Data;
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
        // [SerializeField] private LoadChatUsersUI chatUsersList;
        // [SerializeField] private ToggleButton joinUserToggleButton;
        // [SerializeField] private ToggleButton approveUserToggleButton;
        // [SerializeField] private ToggleButton banUserToggleButton;
        [SerializeField] private DisplayChatRoomInfoUI displayChatRoomInfoUI;
        [SerializeField] private DisplayUserMenuUI displayUserMenuUI;
        // [SerializeField] private TMP_Text userNameText;
        private WindowType windowInfo;
        private string _userName = "Me";
        // private User selectedUser = null;
        // private List<Guid> usersInRoom = new List<Guid>();
        // private List<Guid> approvedUsersInRoom = new List<Guid>();
        // private List<Guid> bannedUsersInRoom = new List<Guid>();

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
            // chatUsersList.onCreatedUserUI += UserUICreated;
            // chatUsersList.onCreatedUserUI += UserUIDestroyed;
            // WebSocketConnection.onReceivedUsersListInRoom += RoomUsersReceived;
            // WebSocketConnection.onReceivedBannedUsersListInRoom += RoomBannedUsersReceived;
            // WebSocketConnection.onReceivedApprovedUsersListInRoom += RoomApprovedUsersReceived;
        }

        // private void RoomUsersReceived((Room room, List<User> users) obj)
        // {
        //     if (windowInfo.IsUser) return;
        //     usersInRoom = new List<Guid>();
        //     if (obj.room.GetGuid() == windowInfo.Room.GetGuid())
        //     {
        //         foreach (var user in obj.users)
        //         {
        //             usersInRoom.Add(user.GetUserGuid());
        //         }
        //     }
        //     UpdateUserOptionsMenu(selectedUser);
        // }
        //
        // private void RoomApprovedUsersReceived((Room room, List<User> users) obj)
        // {
        //     if (windowInfo.IsUser) return;
        //     approvedUsersInRoom = new List<Guid>();
        //     if (obj.room.GetGuid() == windowInfo.Room.GetGuid())
        //     {
        //         foreach (var user in obj.users)
        //         {
        //             approvedUsersInRoom.Add(user.GetUserGuid());
        //         }
        //     }
        //     UpdateUserOptionsMenu(selectedUser);
        // }
        //
        // private void RoomBannedUsersReceived((Room room, List<User> users) obj)
        // {
        //     if (windowInfo.IsUser) return;
        //     bannedUsersInRoom = new List<Guid>();
        //     if (obj.room.GetGuid() == windowInfo.Room.GetGuid())
        //     {
        //         foreach (var user in obj.users)
        //         {
        //             bannedUsersInRoom.Add(user.GetUserGuid());
        //         }
        //     }
        //     UpdateUserOptionsMenu(selectedUser);
        // }
        //
        // private void UserUIDestroyed(DisplayChatUserUI obj)
        // {
        //     obj.onSelectedUser += UserSelected;
        // }
        //
        // private void UserSelected(User obj)
        // {
        //     selectedUser = obj;
        //     UpdateUserOptionsMenu(obj);
        //     WebSocketConnection webSocketConnection = FindObjectOfType<WebSocketConnection>();
        //     webSocketConnection.GetApprovedUsers(windowInfo.Room);
        //     webSocketConnection.GetBannedUsers(windowInfo.Room);
        //     webSocketConnection.AskForUsers(windowInfo.Room);
        // }
        //
        // private void UpdateUserOptionsMenu(User obj)
        // {
        //     joinUserToggleButton.SetIsOn(!usersInRoom.Contains(obj.GetUserGuid()));
        //     banUserToggleButton.SetIsOn(!bannedUsersInRoom.Contains(obj.GetUserGuid()));
        //     approveUserToggleButton.SetIsOn(!approvedUsersInRoom.Contains(obj.GetUserGuid()));
        // }
        //
        // private void UserUICreated(DisplayChatUserUI obj)
        // {
        //     obj.onSelectedUser -= UserSelected;
        // }

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

        // public void ApproveUser()
        // {
        //     if (windowInfo.IsUser || selectedUser == null) return;
        //     
        //     if (approveUserToggleButton.IsOn())
        //     {
        //         FindObjectOfType<WebSocketConnection>().ApproveUser(selectedUser, windowInfo.Room);
        //     }
        //     else
        //     {
        //         FindObjectOfType<WebSocketConnection>().RemoveApproveUser(selectedUser, windowInfo.Room);
        //     }
        //     approveUserToggleButton.Toggle();
        // }
        //
        // public void BanUser()
        // {
        //     if (windowInfo.IsUser || selectedUser == null) return;
        //     if (banUserToggleButton.IsOn())
        //     {
        //         FindObjectOfType<WebSocketConnection>().BanUser(selectedUser, windowInfo.Room);
        //     }
        //     else
        //     {
        //         FindObjectOfType<WebSocketConnection>().UnbanUser(selectedUser, windowInfo.Room);
        //     }
        //     banUserToggleButton.Toggle();
        // }
        //
        // public void AddUser()
        // {
        //     if (windowInfo.IsUser || selectedUser == null) return;
        //     
        //     if (joinUserToggleButton.IsOn())
        //     {
        //         FindObjectOfType<WebSocketConnection>().AddUser(selectedUser, windowInfo.Room);
        //     }
        //     else
        //     {
        //         FindObjectOfType<WebSocketConnection>().RemoveUser(selectedUser, windowInfo.Room);
        //     }
        //     joinUserToggleButton.Toggle();
        // }
    }
}
