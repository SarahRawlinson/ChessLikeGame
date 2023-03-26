using TMPro;
using UnityEngine;

namespace Multiplayer.View.DisplayData
{
    public class DisplayChatUserUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text user;

        public void SetUser(string userText)
        {
            user.text = userText;
        }
    }
}
