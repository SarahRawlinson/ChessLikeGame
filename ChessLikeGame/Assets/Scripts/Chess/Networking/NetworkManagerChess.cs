using System;
using System.Collections.Generic;
using Chess.Movement;
using EndGame;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chess.Networking
{
    public class NetworkManagerChess : NetworkManager
    {
        [Tooltip("This is the the start of the scene name in a live game eg 'Map' for the actual name of Map_01")]
        [SerializeField] private string startOfSceneName = "Space_Solar_System";
        [SerializeField] string startScene;
        [SerializeField] private GameObject unitBasePrefab = null;
        [SerializeField] private EndGameHandler endGameHandlerPrefab = null;
        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        public List<NetworkPlayerChess> Players { get; } = new List<NetworkPlayerChess>();
        private bool isGameInProgress = false;

        #region Server

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (!isGameInProgress) return;
            conn.Disconnect();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            NetworkPlayerChess player = conn.identity.GetComponent<NetworkPlayerChess>();
            Players.Add(player);
            player.SetDisplayName($"Player {numPlayers}");

            player.SetDisplayColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
            //Debug.Log($"Player {player.DisplayName} Has Connected");
            //player.RpcLogTest();
            player.SetPartyOwner(Players.Count == 1);
        }

        private void SpawnStartObjects(NetworkPlayerChess player)
        {
            if (player == null) return;
            Transform startPosition = GetStartPosition();
            GameObject unitSpawnerInstance = Instantiate(unitBasePrefab, startPosition.position, Quaternion.identity);
            NetworkServer.Spawn(unitSpawnerInstance, player.connectionToClient);
            player.transform.position = startPosition.position;
        }

        public override void OnStopServer()
        {
            Players.Clear();
            isGameInProgress = false;
        }

        public void StartGame()
        {
            if (Players.Count < 2) return;
            isGameInProgress = true;
            ServerChangeScene(startScene);
            
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            NetworkPlayerChess player = conn.identity.GetComponent<NetworkPlayerChess>();
            Players.Remove(player);
            base.OnServerDisconnect(conn);
        }

        // remeber this string refference must stay consistant
        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith(startOfSceneName))
            {
                EndGameHandler endGameHandlerInstance = Instantiate(endGameHandlerPrefab);
                NetworkServer.Spawn(endGameHandlerInstance.gameObject);

                foreach (NetworkPlayerChess player in Players)
                {
                    SpawnStartObjects(player);
                    player.GetComponent<CameraController>().SceneLoaded();
                }
                
            }            
        }

        #endregion

        #region Client

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            ClientOnConnected?.Invoke();

        }
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            ClientOnDisconnected?.Invoke();
        }

        public override void OnStopClient()
        {
            Players.Clear();
        }

        #endregion



    }
}
