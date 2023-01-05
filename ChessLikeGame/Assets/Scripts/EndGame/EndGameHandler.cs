using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace EndGame
{
    public class EndGameHandler : NetworkBehaviour
    {
        public static event Action<string> ClientOnGameOver;
        public static event Action ServerOnGameOver;
        private List<EndGameObject> endGames = new List<EndGameObject>();
        private List<int> endGamePlayers = new List<int>();
        #region Server

        public override void OnStartServer()
        {
            EndGameObject.ServerIEndGameSpawned += ServerHandleEndGameSpawned;
            EndGameObject.ServerIEndGameDespawned += ServerHandleEndGameDespawned;
        }

        public override void OnStopServer()
        {
            EndGameObject.ServerIEndGameSpawned -= ServerHandleEndGameSpawned;
            EndGameObject.ServerIEndGameDespawned -= ServerHandleEndGameDespawned;
        }

        [Server]
        private void ServerHandleEndGameSpawned(EndGameObject endGame)
        {
            Debug.Log("End Game Object Has Spawned");
            endGames.Add(endGame);
            int player = endGame.connectionToClient.connectionId;
            if (endGamePlayers.Contains(player)) return;
            endGamePlayers.Add(player);
        }

        [Server]
        private void ServerHandleEndGameDespawned(EndGameObject endGame)
        {
            int player = endGame.connectionToClient.connectionId;
            endGames.Remove(endGame);
            Debug.Log("End Game Object Destroyed");
            foreach (EndGameObject obj in endGames)
            {
                if (obj.connectionToClient.connectionId == player) return;
            }
            endGamePlayers.Remove(player);
            if (endGamePlayers.Count != 1) return;
            Debug.Log("Game Over");
            player = endGamePlayers[0];
            RpcGameOver($"Player {player}");
            ServerOnGameOver?.Invoke();
        }

        #endregion

        #region Client


        [ClientRpc]
        private void RpcGameOver(string winner)
        {
            ClientOnGameOver?.Invoke(winner);
        }


        #endregion
    }
}