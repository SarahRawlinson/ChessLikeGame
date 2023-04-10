﻿using System;
using LibObjects;
using Multiplayer.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.View.DisplayData
{
    public class DisplayGameRoomUI : MonoBehaviour
    {
        [FormerlySerializedAs("_tmpText")] [SerializeField] private TMP_Text _roomNameText;
        private Guid id;
        private Room _room;
        private static event Action<Guid> onJoinRoom; 
        public void UpdateHostInfo(Room room)
        {
            _room = room;
            _roomNameText.text = room.GetGuid().ToString();
            id = room.GetGuid();
        }

        public void Join()
        {
            FindObjectOfType<WebSocketConnection>().JoinRoom(_room.GetGuid());
            onJoinRoom?.Invoke(id);
        }
        
    }
}