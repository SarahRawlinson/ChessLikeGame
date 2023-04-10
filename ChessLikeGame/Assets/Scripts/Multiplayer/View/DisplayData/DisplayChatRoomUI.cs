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
    public class DisplayChatRoomUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomString;
        [SerializeField] private ToggleButton _button;
        [SerializeField] private GameObject openButton;
        private List<User> _users;
        private Room _room;

        public void SetRoom(Room room)
        {
            _room = room;
            roomString.text = room.GetRoomName();
            WebSocketConnection.onChatRoomList += UpdateUsers;
            WebSocketConnection.onReceivedUsersListInRoom += CheckIfUsersInRoom;
            var webSocketConnection = FindObjectOfType<WebSocketConnection>();
            if (_room.GetCreator() == webSocketConnection.GetClientUser().GetUserName() )
            {
                _button.SetIsOn(false);
                openButton.SetActive(true);
                return;
            }
            _button.SetIsOn(true);
            openButton.SetActive(false);
            AskForUsersOfRoom(webSocketConnection);
        }

        private void UpdateUsers(List<Room> obj)
        {
            AskForUsersOfRoom(FindObjectOfType<WebSocketConnection>());
        }

        public void ShowWindow()
        {
            FindObjectOfType<HandleChat>().OpenWindow(_room);
        }

        public void AskForUsersOfRoom(WebSocketConnection webSocketConnection)
        {
            webSocketConnection.AskForUsers(_room);
        }

        private void CheckIfUsersInRoom((Room room, List<User> users) obj)
        {
            Debug.Log("checking users in room");
            User thisUser = FindObjectOfType<WebSocketConnection>().GetClientUser();
            if (obj.room.GetGuid() == _room.GetGuid())
            {
                // Debug.Log($"Room is {_room.GetRoomName()}");
                _users = obj.users;
                foreach (var user in obj.users)
                {
                    Debug.Log($"Room has user {user.GetUserName()}");
                    if (user.GetUserName() == thisUser.GetUserName())
                    {
                        Debug.Log($"this user {user.GetUserName()} in room {_room.GetRoomName()}");
                        _button.SetIsOn(false);
                        openButton.SetActive(true);
                        return;
                    }
                }
                openButton.SetActive(false);
                _button.SetIsOn(true);
            }
        }

        public void PressChatToggleButton()
        {
            if (_button.IsOn())
            {
                FindObjectOfType<HandleChat>().StartNewChatWithRoom(_room);
            }
            else
            {
                FindObjectOfType<HandleChat>().LeaveChat(_room);
            }
            
        }

        private void OnDestroy()
        {
            WebSocketConnection.onChatRoomList -= UpdateUsers;
            WebSocketConnection.onReceivedUsersListInRoom -= CheckIfUsersInRoom;
        }
    }
}