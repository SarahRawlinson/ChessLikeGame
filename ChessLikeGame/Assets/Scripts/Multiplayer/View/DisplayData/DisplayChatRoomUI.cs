using LibObjects;
using Multiplayer.View.LoadData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.View.DisplayData
{
    public class DisplayChatRoomUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomString;
        private Room _room;

        public void SetRoom( Room room)
        {
            _room = room;
            roomString.text = room.RoomID.ToString();
        }
        
        public void StartChat()
        {
            FindObjectOfType<HandleChat>().StartNewChatWithRoom(_room);
        }
    }
}