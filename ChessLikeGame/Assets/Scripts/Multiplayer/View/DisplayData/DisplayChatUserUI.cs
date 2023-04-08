using System;
using LibObjects;
using Multiplayer.View.LoadData;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.View.DisplayData
{
    public class DisplayChatUserUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text userText;
        private User _user;
        public event Action<User> onSelectedUser; 

        public void SetUser(User user)
        {
            _user = user;
            userText.text = user.GetUserName();
        }

        public void SelectedEvent()
        {
            onSelectedUser?.Invoke(_user);
        }

        public User GetUser()
        {
            return _user;
        }
    }
}
