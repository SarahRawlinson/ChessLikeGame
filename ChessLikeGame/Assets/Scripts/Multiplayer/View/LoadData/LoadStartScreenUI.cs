using LibObjects;
using MessageServer.Data;
using Multiplayer.Controllers;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class LoadStartScreenUI : MonoBehaviour
    {
        [SerializeField] private GameObject loginDisplay;
        [SerializeField] private GameObject hostGameDisplay;
        [SerializeField] private GameObject joinGameDisplay;
        [SerializeField] private GameObject loginButton;
        [SerializeField] private GameObject hostGameButton;
        [SerializeField] private GameObject joinGameButton;
        [SerializeField] private GameObject startMenuDisplay;
        [SerializeField] private GameObject chatMenuDisplay;

        private void Start()
        {
            WebSocketConnection.onAuthenicate += Authenticated;
        }

        public void Authenticated(bool on)
        {
            hostGameButton.SetActive(on);
            joinGameButton.SetActive(on);
            loginButton.SetActive(!on);
            chatMenuDisplay.SetActive(on);
        }
        public void ShowDisplay()
        {
            startMenuDisplay.SetActive(true);
        }
    
        public void HideDisplay()
        {
            startMenuDisplay.SetActive(false);
        }

        public void Login()
        {
            Debug.Log("login show");
            loginDisplay.SetActive(true);
        }

        public void HostGame()
        {
            hostGameDisplay.SetActive(true);
        }

        public void JoinGame()
        {
            joinGameDisplay.SetActive(true);
        }
    }
}
