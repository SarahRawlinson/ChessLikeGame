using System;
using TMPro;
using UnityEngine;

namespace Multiplayer.View.DisplayData
{
    public class DisplayChatMessageUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text user;
        [SerializeField] private TMP_Text message;
        [SerializeField] private TMP_Text time;

        public void SetMessage(string userText, string messageText)
        {
            user.text = userText;
            message.text = messageText;
            time.text = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
        }
        
    }
}