using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.View.DisplayData
{
    public class DisplayChatRoomUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text room;

        public void SetRoom(string userText)
        {
            room.text = userText;
        }
    }
}