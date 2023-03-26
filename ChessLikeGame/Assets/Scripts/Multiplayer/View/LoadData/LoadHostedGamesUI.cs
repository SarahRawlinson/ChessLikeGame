using System.Collections.Generic;
using System.Linq;
using Multiplayer.Controllers;
using Multiplayer.Models.Connection;
using Multiplayer.View.DisplayData;
using Multiplayer.View.UI;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class LoadHostedGamesUI : MonoBehaviour
    {
        [SerializeField] private ScrollContentUI _scrollContentUI;
        [SerializeField] private DisplayHostUI _gameObjectPrefab;
        [SerializeField] private GameObject displayPanel;
        private List<User> _users = new List<User>();
        private List<DisplayHostUI> _usersUI = new List<DisplayHostUI>();

        private void Start()
        {
            WebSocketConnection.onHostsList += ProcessHosts;
        }

        private void ProcessHosts(string obj)
        {
            string[] ls = obj.Split(":");
            List<string> activeUser = new List<string>();

            for (var index = _users.Count -1; index >= 0; index--)
            {
                var user = _users[index];
                if (ls.Contains(user.Username))
                {
                    activeUser.Add(user.Username);
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
                    AddHost(new User(u));
                    
                }
            }
        }

        public void HideDisplay()
        {
            displayPanel.SetActive(false);
        }
        
        public void ShowDisplay()
        {
            FindObjectOfType<WebSocketConnection>().GetRoomList();
            displayPanel.SetActive(true);
        }
        
        public void AddHost(User user)
        {
            _users.Add(user);
            GameObject gObject = _scrollContentUI.AddContent(_gameObjectPrefab.gameObject);
            DisplayHostUI ui = gObject.GetComponent<DisplayHostUI>();
            ui.UpdateText(user.Username);
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
                    DisplayHostUI ui = _usersUI[index];
                    _usersUI.Remove(ui);
                    Destroy(ui.gameObject);
                }
            }
        }
        
    }
}