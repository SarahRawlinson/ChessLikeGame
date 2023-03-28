using System.Collections.Generic;
using System.Linq;
using MessageServer.Data;
using Multiplayer.Controllers;
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
        private List<Room> _rooms = new List<Room>();
        private List<DisplayHostUI> _roomUI = new List<DisplayHostUI>();

        private void Start()
        {
            WebSocketConnection.onHostsList += ProcessHosts;
        }

        private void ProcessHosts(List<Room> obj)
        {
            List<Room> ls = new List<Room>();
            foreach (var room in obj)
            {
                ls.Add(room);
            }
            List<Room> activeRooms = new List<Room>();

            for (var index = _rooms.Count -1; index >= 0; index--)
            {
                var room = _rooms[index];
                if (ls.Contains(room))
                {
                    activeRooms.Add(room);
                }
                else
                {
                    RemoveHost(room);
                }
            }

            foreach (var u in ls)
            {
                if (!activeRooms.Contains(u))
                {
                    AddHost(u);
                    
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
        
        public void AddHost(Room room)
        {
            _rooms.Add(room);
            GameObject gObject = _scrollContentUI.AddContent(_gameObjectPrefab.gameObject);
            DisplayHostUI ui = gObject.GetComponent<DisplayHostUI>();
            ui.UpdateHostInfo(room);
            _roomUI.Add(ui);
        }
        
        public void RemoveHost(Room user)
        {
            for (var index = 0; index < _rooms.Count; index++)
            {
                var u = _rooms[index];
                if (Equals(u, user))
                {
                    _rooms.Remove(u);
                    DisplayHostUI ui = _roomUI[index];
                    _roomUI.Remove(ui);
                    Destroy(ui.gameObject);
                }
            }
        }
        
    }
}