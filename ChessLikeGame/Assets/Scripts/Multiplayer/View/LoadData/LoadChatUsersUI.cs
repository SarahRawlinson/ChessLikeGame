using System;
using System.Collections;
using System.Collections.Generic;
using LibObjects;
using Multiplayer.Controllers;
using Multiplayer.View.DisplayData;
using Multiplayer.View.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class LoadChatUsersUI : MonoBehaviour
    {
        [SerializeField] private ScrollContentUI _scrollContentUI;
        [SerializeField] private DisplayChatUserUI _gameObjectPrefab;
        [SerializeField] private bool AllUsers = false;
        private List<User> _users = new List<User>();
        private List<DisplayChatUserUI> _usersUI = new List<DisplayChatUserUI>();
        public event Action<DisplayChatUserUI> onCreatedUserUI; 
        public event Action<DisplayChatUserUI> onDestroyedUserUI;
        

        private void Start()
        {
            if (AllUsers)
            {
                WebSocketConnection.onUsersList += ProcessUsers;
            }
        }

        public void ProcessUsers(List<User> obj)
        {
            // Debug.Log("add users");
            var ls = new List<User>();
            for (var index = 0; index < obj.Count; index++)
            {
                var user = obj[index];
                ls.Add(user);
            }

            List<User> activeUser = new List<User>();
            
            for (var index = _users.Count -1; index >= 0; index--)
            {
                var user = _users[index];
                if (((IList) ls).Contains(user))
                {
                    activeUser.Add(user);
                }
                else
                {
                    RemoveHost(user);
                }
            }

            for (var index = 0; index < ls.Count; index++)
            {
                var u = ls[index];
                if (!activeUser.Contains(u))
                {
                    AddHost(u);
                }
            }
        }
        
        public void RefreshUsers()
        {
            if (AllUsers)
            {
                Debug.Log("refreshed users button pressed");
                FindObjectOfType<WebSocketConnection>().RefreshUsers();
            }
            
        }

        public void AddHost(User user)
        {
            // Debug.Log($"added {user.GetUserName()}");
            _users.Add(user);
            GameObject gObject = _scrollContentUI.AddContent(_gameObjectPrefab.gameObject);
            DisplayChatUserUI ui = gObject.GetComponent<DisplayChatUserUI>();
            ui.SetUser(user);
            _usersUI.Add(ui);
            onCreatedUserUI?.Invoke(ui);
        }

        public List<DisplayChatUserUI> GetUsersUI()
        {
            return _usersUI;
        }

        public void RemoveHost(User user)
        {
            for (var index = 0; index < _users.Count; index++)
            {
                var u = _users[index];
                if (Equals(u, user))
                {
                    Debug.Log($"removed {user.GetUserName()}");
                    DisplayChatUserUI ui = _usersUI[index];
                    onDestroyedUserUI?.Invoke(ui);
                    _users.Remove(u);
                    _usersUI.Remove(ui);
                    Destroy(ui.gameObject);
                }
            }
        }

        private void OnDestroy()
        {
            if (AllUsers)
            {
                WebSocketConnection.onUsersList -= ProcessUsers;
            }
        }
    }
}