using Multiplayer.View.UI;
using UnityEngine;

namespace Multiplayer.View.DisplayData
{
    public class DisplayUsersChatUI : MonoBehaviour
    {
        [SerializeField] private DisplayChatUserUI _userUIPrefab;
        [SerializeField] private ScrollContentUI _scrollContentUI;

        public void SendMessageToUI(string user)
        {
            GameObject o = _scrollContentUI.AddContent(_userUIPrefab.gameObject);
            DisplayChatUserUI userUI = o.GetComponent<DisplayChatUserUI>();
            userUI.SetUser(user);
        }
        
    }
}
