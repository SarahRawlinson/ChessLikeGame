using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.UI
{
    public class MainMenuDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject landingPage = null;

        [SerializeField] private bool useSteam = false;
        [SerializeField] Toggle useSteamToggle;
        [SerializeField] GameObject steamNetworker;
        [SerializeField] GameObject mirrorNetworker;
        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinedRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;

        private void Start()
        {
            // useSteamToggle.isOn = useSteam;
            // if (useSteam && (!steamNetworker.activeSelf || mirrorNetworker.activeSelf)) Debug.Log("Error in Networking Set up");
            // if (!useSteam && (steamNetworker.activeSelf || !mirrorNetworker.activeSelf)) Debug.Log("Error in Networking Set up");
            // if (!useSteam) return;
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinedRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinedRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnlobbyEntered);
        }
        public void HostLobby()
        {
            landingPage.SetActive(false);
            if (useSteam)
            {
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
                return;
            }
            StartNetworkManager();
        }

        private static void StartNetworkManager()
        {
            NetworkManager.singleton.StartHost();
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                landingPage.SetActive(true);
                return;
            }
            StartNetworkManager();
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress", SteamUser.GetSteamID().ToString());
        }

        private void OnLobbyJoinedRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnlobbyEntered(LobbyEnter_t callback)
        {
            if (NetworkServer.active) return;
            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");
            NetworkManager.singleton.networkAddress = hostAddress;
            NetworkManager.singleton.StartClient();
            landingPage.SetActive(false);
        }
    }
}
