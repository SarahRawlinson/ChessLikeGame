using System.Collections;
using System.Collections.Generic;
using MessageServer.Data;
using Multiplayer.Controllers;
using Multiplayer.Models.Connection;
using Multiplayer.View.DisplayData;
using Multiplayer.View.UI;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class LoadChatRoomsUI : MonoBehaviour
    {
        [SerializeField] private ScrollContentUI _scrollContentUI;
        [SerializeField] private DisplayChatRoomUI _gameObjectPrefab;
        private List<string> _rooms = new List<string>();
        private List<DisplayChatRoomUI> _roomsUI = new List<DisplayChatRoomUI>();

        private void Start()
        {
            WebSocketConnection.onHostsList += ProcessHosts;
        }
        

        private void ProcessHosts(List<Room> obj)
        {
            List<string> ls = new List<string>();
            obj.ForEach(a => ls.Add(a.roomKey));
            List<string> activeUser = new List<string>();
            
            for (var index = _rooms.Count -1; index >= 0; index--)
            {
                var user = _rooms[index];
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
        public void AddHost(string room)
        {
            _rooms.Add(room);
            GameObject gObject = _scrollContentUI.AddContent(_gameObjectPrefab.gameObject);
            DisplayChatRoomUI ui = gObject.GetComponent<DisplayChatRoomUI>();
            ui.SetRoom(room);
            _roomsUI.Add(ui);
        }
        
        public void RemoveHost(string room)
        {
            for (var index = 0; index < _rooms.Count; index++)
            {
                var u = _rooms[index];
                if (Equals(u, room))
                {
                    _rooms.Remove(u);
                    DisplayChatRoomUI ui = _roomsUI[index];
                    _roomsUI.Remove(ui);
                    Destroy(ui.gameObject);
                }
            }
        }
    }
}