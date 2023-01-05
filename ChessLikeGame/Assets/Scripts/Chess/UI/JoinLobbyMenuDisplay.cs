using Chess.Networking;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.UI
{
    public class JoinLobbyMenuDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject landingPage = null;
        [SerializeField] private TMP_InputField addressImput = null;
        [SerializeField] private Button joinButton = null;

        private void OnEnable()
        {
            NetworkManagerChess.ClientOnConnected += HandleClientConnected;
            NetworkManagerChess.ClientOnDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            NetworkManagerChess.ClientOnConnected -= HandleClientConnected;
            NetworkManagerChess.ClientOnDisconnected -= HandleClientDisconnected;
        }

        public void Join()
        {
            string address = addressImput.text;
            NetworkManager.singleton.networkAddress = address;
            NetworkManager.singleton.StartClient();
            joinButton.interactable = false;
        }

        private void HandleClientConnected()
        {
            joinButton.interactable = true;
            landingPage.SetActive(false);
            gameObject.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            joinButton.interactable = true;
            landingPage.SetActive(true);
            gameObject.SetActive(true);
        }
    }
}
