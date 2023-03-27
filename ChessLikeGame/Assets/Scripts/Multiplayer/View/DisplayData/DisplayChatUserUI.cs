using MessageServer.Data;
using Multiplayer.View.LoadData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.View.DisplayData
{
    public class DisplayChatUserUI : MonoBehaviour
    {
        [FormerlySerializedAs("user")] [SerializeField] private TMP_Text userText;
        private User _user;

        public void SetUser(User user)
        {
            _user = user;
            userText.text = user.GetUserName();
        }

        public void StartChat()
        {
            FindObjectOfType<HandleChat>().StartNewChatWithUser(_user);
        }
    }
}
