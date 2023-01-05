using System.Collections.Generic;
using Chess.Networking;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Chess.UI
{
    public class LobbyMenuDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUI = null;
        [SerializeField] private Image[] connectedIcons = new Image[4];
        [SerializeField] private Button startGameButton = null;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];

        private void Start()
        {
            NetworkManagerChess.ClientOnConnected += HandleClientConnected;
            NetworkManagerChess.ClientOnDisconnected += HandleClientDisconnected;
            NetworkPlayerChess.AuthorityOnPartOwnerStateUpdated += AuthorityHandlePartOwnerStateUpdated;
            NetworkPlayerChess.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        }

        private void AuthorityHandlePartOwnerStateUpdated(bool obj)
        {
            startGameButton.gameObject.SetActive(obj);
        }

        private void OnDestroy()
        {
            NetworkManagerChess.ClientOnConnected -= HandleClientConnected;
            NetworkManagerChess.ClientOnDisconnected -= HandleClientDisconnected;
            NetworkPlayerChess.AuthorityOnPartOwnerStateUpdated -= AuthorityHandlePartOwnerStateUpdated;
            NetworkPlayerChess.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        }

        private void ClientHandleInfoUpdated()
        {
            List<NetworkPlayerChess> players = ((NetworkManagerChess)NetworkManager.singleton).Players;

            for (int i = 0; i < players.Count; i++)
            {
                playerNameTexts[i].text = players[i].DisplayName;
                connectedIcons[i].color = players[i].DisplayColor;
                connectedIcons[i].gameObject.SetActive(true);
            }
            for (int i = players.Count; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting For Player...";
                connectedIcons[i].color = Color.white;
                connectedIcons[i].gameObject.SetActive(false);
            }
            startGameButton.interactable = players.Count >= 2;
        }

        private void HandleClientDisconnected()
        {
            
        }

        public void StartGame()
        {
            NetworkClient.connection.identity.GetComponent<NetworkPlayerChess>().CmdStartGame();
        }

        private void HandleClientConnected()
        {
            lobbyUI.SetActive(true);
        }

        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
                SceneManager.LoadScene(0);
            }
        }
    }
}
