using System.Collections;
using System.Collections.Generic;
using MessageServer.Data;
using Multiplayer.Controllers;
using Multiplayer.View.DisplayData;
using Multiplayer.View.UI;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class LoadChatUsersUI : MonoBehaviour
    {
        [SerializeField] private ScrollContentUI _scrollContentUI;
        [SerializeField] private DisplayChatUserUI _gameObjectPrefab;
        private List<User> _users = new List<User>();
        private List<DisplayChatUserUI> _usersUI = new List<DisplayChatUserUI>();

        private void Start()
        {
            WebSocketConnection.onUsersList += ProcessUsers;
        }

        private void ProcessUsers(List<User> obj)
        {
            var ls = new List<User>();
            foreach (var user in obj)
            {
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
            
            foreach (var u in ls)
            {
                if (!activeUser.Contains(u))
                {
                    AddHost(u);
                    
                }
            }
        }

        public void AddHost(User user)
        {
            _users.Add(user);
            GameObject gObject = _scrollContentUI.AddContent(_gameObjectPrefab.gameObject);
            DisplayChatUserUI ui = gObject.GetComponent<DisplayChatUserUI>();
            ui.SetUser(user);
            _usersUI.Add(ui);
        }
        
        public void RemoveHost(User user)
        {
            for (var index = 0; index < _users.Count; index++)
            {
                var u = _users[index];
                if (Equals(u, user))
                {
                    _users.Remove(u);
                    DisplayChatUserUI ui = _usersUI[index];
                    _usersUI.Remove(ui);
                    Destroy(ui.gameObject);
                }
            }
        }
    }
}