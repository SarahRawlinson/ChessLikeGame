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
            WebSocketConnection.onReceivedUsersListInRoom += CheckIfUsersInRoom;
            var webSocketConnection = FindObjectOfType<WebSocketConnection>();
            if (_room.GetCreator().GetUserName() == webSocketConnection.GetClientUser().GetUserName() )
            {
                _button.SetIsOn(false);
                openButton.SetActive(true);
                return;
            }
            _button.SetIsOn(true);
            openButton.SetActive(false);
            AskForUsersOfRoom(webSocketConnection);
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
            User thisUser = FindObjectOfType<WebSocketConnection>().GetClientUser();
            if (obj.room.GetGuid() == _room.GetGuid())
            {
                _users = obj.users;
                foreach (var user in obj.users)
                {
                    if (user.GetUserName() == thisUser.GetUserName())
                    {
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
    }
}