using System;
using Chess.Stats;
using Mirror;
using UnityEngine;

namespace EndGame
{
    public class EndGameObject : NetworkBehaviour
    {
        [SerializeField] private Health health;

        public static event Action<int> ServerOnPlayerDie;
        public static event Action<EndGameObject> ServerIEndGameSpawned;
        public static event Action<EndGameObject> ServerIEndGameDespawned;

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
            ServerIEndGameSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerIEndGameDespawned?.Invoke(this);
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client





        #endregion
    }
}