using System.Collections;
using System.Collections.Generic;
using LibObjects;
using Multiplayer.Controllers;
using Multiplayer.View.DisplayData;
using Multiplayer.View.UI;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class LoadChatRoomsUI : MonoBehaviour
    {
        [SerializeField] private ScrollContentUI _scrollContentUI;
        [SerializeField] private DisplayChatRoomUI _gameObjectPrefab;
        private List<Room> _rooms = new List<Room>();
        private List<DisplayChatRoomUI> _roomsUI = new List<DisplayChatRoomUI>();

        private void Start()
        {
            WebSocketConnection.onGameRoomList += ProcessHosts;
            WebSocketConnection.onRoomDestroyed += RoomDestroyed;
        }
        
        private void OnDestroy()
        {
            WebSocketConnection.onGameRoomList -= ProcessHosts;
            WebSocketConnection.onRoomDestroyed -= RoomDestroyed;
        }

        private void RoomDestroyed(Room obj)
        {
            foreach (var room in _rooms)
            {
                if (obj.GetGuid() == room.GetGuid())
                {
                    RemoveHost(room);
                    return;
                }
            }
        }

        public void RefreshRooms()
        {
            Debug.Log("refreshed rooms button pressed");
            FindObjectOfType<WebSocketConnection>().RefreshRooms();
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
        public void AddHost(Room room)
        {
            _rooms.Add(room);
            GameObject gObject = _scrollContentUI.AddContent(_gameObjectPrefab.gameObject);
            DisplayChatRoomUI ui = gObject.GetComponent<DisplayChatRoomUI>();
            ui.SetRoom(room);
            _roomsUI.Add(ui);
        }
        
        public void RemoveHost(Room room)
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