using System;
using System.Collections.Generic;
using LibObjects;
using MessageServer.Data;
using Multiplayer.Controllers;
using Multiplayer.View.LoadData;
using Multiplayer.View.UI;
using TMPro;
using UnityEngine;

namespace Multiplayer.View.DisplayData
{
    public class DisplayUserMenuUI : MonoBehaviour
    {
        [SerializeField] private ToggleButton joinUserToggleButton;
        [SerializeField] private ToggleButton approveUserToggleButton;
        [SerializeField] private ToggleButton banUserToggleButton;
        [SerializeField] private LoadChatUsersUI chatUsersList;
        [SerializeField] private TMP_Text userNameText;
        
        private List<Guid> usersInRoom = new List<Guid>();
        private List<Guid> approvedUsersInRoom = new List<Guid>();
        private List<Guid> bannedUsersInRoom = new List<Guid>();
        private Room _room = null;
        private User selectedUser = null;

        public void SetRoom(Room room)
        {
            _room = room;
            AskForUpdates();
        }
        
        private void Awake()
        {
            chatUsersList.onCreatedUserUI += UserUICreated;
            chatUsersList.onDestroyedUserUI += UserUIDestroyed;
            WebSocketConnection.onReceivedUsersListInRoom += RoomUsersReceived;
            WebSocketConnection.onReceivedBannedUsersListInRoom += RoomBannedUsersReceived;
            WebSocketConnection.onReceivedApprovedUsersListInRoom += RoomApprovedUsersReceived;
        }
        
        private void OnDestroy()
        {
            chatUsersList.onCreatedUserUI -= UserUICreated;
            chatUsersList.onDestroyedUserUI -= UserUIDestroyed;
            WebSocketConnection.onReceivedUsersListInRoom -= RoomUsersReceived;
            WebSocketConnection.onReceivedBannedUsersListInRoom -= RoomBannedUsersReceived;
            WebSocketConnection.onReceivedApprovedUsersListInRoom -= RoomApprovedUsersReceived;
        }
        
        private void RoomUsersReceived((Room room, List<User> users) obj)
        {
            if (_room == null) return;
            usersInRoom.Clear();
            if (obj.room.GetGuid() == _room.GetGuid())
            {
                Debug.Log($"{obj.room.GetRoomName()} == this room");
                foreach (var user in obj.users)
                {
                    Debug.Log($"{obj.room.GetRoomName()} contains {user.GetUserName()}");
                    usersInRoom.Add(user.GetUserGuid());
                    Debug.Log($"{user.GetUserName()} was added {user.GetUserGuid().ToString()}");
                }
            }
            UpdateUserOptionsMenu(selectedUser);
        }
        
        private void RoomApprovedUsersReceived((Room room, List<User> users) obj)
        {
            if (_room == null) return;
            approvedUsersInRoom.Clear();
            if (obj.room.GetGuid() == _room.GetGuid())
            {
                foreach (var user in obj.users)
                {
                    approvedUsersInRoom.Add(user.GetUserGuid());
                }
            }
            UpdateUserOptionsMenu(selectedUser);
        }
        
        private void RoomBannedUsersReceived((Room room, List<User> users) obj)
        {
            if (_room == null) return;
            bannedUsersInRoom.Clear();
            if (obj.room.GetGuid() == _room.GetGuid())
            {
                foreach (var user in obj.users)
                {
                    bannedUsersInRoom.Add(user.GetUserGuid());
                }
            }
            UpdateUserOptionsMenu(selectedUser);
        }

        private void UserUIDestroyed(DisplayChatUserUI obj)
        {
            obj.onSelectedUser -= UserSelected;
        }

        private void UserSelected(User obj)
        {
            selectedUser = obj;
            UpdateUserOptionsMenu(obj);
            AskForUpdates();
        }

        public void AskForUpdates()
        {
            if (_room == null) return;
            WebSocketConnection webSocketConnection = FindObjectOfType<WebSocketConnection>();
            webSocketConnection.AskForUsers(_room);
            webSocketConnection.GetApprovedUsers(_room);
            webSocketConnection.GetBannedUsers(_room);
        }

        private void UpdateUserOptionsMenu(User obj)
        {
            userNameText.text = obj.GetUserName();
            joinUserToggleButton.SetIsOn(!usersInRoom.Contains(obj.GetUserGuid()));
            banUserToggleButton.SetIsOn(!bannedUsersInRoom.Contains(obj.GetUserGuid()));
            approveUserToggleButton.SetIsOn(!approvedUsersInRoom.Contains(obj.GetUserGuid()));
        }

        private void UserUICreated(DisplayChatUserUI obj)
        {
            obj.onSelectedUser += UserSelected;
        }
        
        public void ApproveUser()
        {
            if (_room == null) return;
            if (approveUserToggleButton.IsOn())
            {
                FindObjectOfType<WebSocketConnection>().ApproveUser(selectedUser, _room);
            }
            else
            {
                FindObjectOfType<WebSocketConnection>().RemoveApproveUser(selectedUser, _room);
            }
            approveUserToggleButton.Toggle();
        }
        
        public void BanUser()
        {
            if (_room == null) return;
            if (banUserToggleButton.IsOn())
            {
                FindObjectOfType<WebSocketConnection>().BanUser(selectedUser, _room);
            }
            else
            {
                FindObjectOfType<WebSocketConnection>().UnbanUser(selectedUser, _room);
            }
            banUserToggleButton.Toggle();
        }
        
        public void AddUser()
        {
            if (_room == null) return;
            if (joinUserToggleButton.IsOn())
            {
                FindObjectOfType<WebSocketConnection>().AddUser(selectedUser, _room);
            }
            else
            {
                FindObjectOfType<WebSocketConnection>().RemoveUser(selectedUser, _room);
            }
            joinUserToggleButton.Toggle();
        }

    }
}