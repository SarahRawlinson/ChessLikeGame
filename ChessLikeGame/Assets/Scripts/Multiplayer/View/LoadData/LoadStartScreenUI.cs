using LibObjects;
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
        [SerializeField] private GameObject logOutButton;
        [SerializeField] private GameObject startMenuDisplay;
        [SerializeField] private GameObject chatMenuDisplay;

        private void Start()
        {
            WebSocketConnection.onAuthenicate += Authenticated;
            //StartGameScreenUI.onGameStarted += HideOnStartGame;
            MultiplayerDirector.NetGameStarted += HideOnStartGame;
        }

        public void HideOnStartGame()
        {
            Debug.Log("Hide Displays");
            hostGameDisplay.SetActive(false);
            joinGameDisplay.SetActive(false);
            startMenuDisplay.SetActive(false);
            
        }

        private void OnDestroy()
        {
            WebSocketConnection.onAuthenicate -= Authenticated;
            //StartGameScreenUI.onGameStarted -= HideOnStartGame;
            MultiplayerDirector.NetGameStarted -= HideOnStartGame;
        }

        public void Authenticated(bool on)
        {
            hostGameButton.SetActive(on);
            joinGameButton.SetActive(on);
            loginButton.SetActive(!on);
            chatMenuDisplay.SetActive(on);
            logOutButton.SetActive(on);
        }
        public void ShowDisplay()
        {
            startMenuDisplay.SetActive(true);
        }

        public void LogOut()
        {
            Authenticated(false);
            FindObjectOfType<WebSocketConnection>().CloseConnection();
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
