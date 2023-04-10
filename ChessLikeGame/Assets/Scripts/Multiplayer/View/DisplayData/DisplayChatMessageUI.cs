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

        public void SetMessage(string userText, string messageText, bool isPrivate=false)
        {
            user.text = userText;
            message.text = messageText;
            if (isPrivate)
            {
                message.fontStyle = FontStyles.Italic;
            }
            time.text = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
        }
        
    }
}