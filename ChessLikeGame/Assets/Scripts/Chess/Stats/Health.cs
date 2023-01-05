using System;
using EndGame;
using Mirror;

namespace Chess.Stats
{
    public class Health: NetworkBehaviour
    {
        public event Action ServerOnDie;
        public override void OnStartServer()
        {
            EndGameObject.ServerOnPlayerDie += ServerHandlePlayerDie;
        }

        public override void OnStopServer()
        {
            EndGameObject.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }
        public void ServerHandlePlayerDie(int connectionId)
        {
            if (connectionId != connectionToClient.connectionId) return;
            DealDamage();
        }
        [Server]
        public void DealDamage()
        {
            ServerOnDie?.Invoke();
        }
    }
}