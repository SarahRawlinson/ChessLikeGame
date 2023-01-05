using Chess.Networking;
using Chess.Pieces;
using Mirror;
using UnityEngine;

namespace Chess.Control
{
    public class RTSUnitSpawner: NetworkBehaviour
    {
        [SerializeField] private ChessPiece[] unitPrefabs;
        public ChessPiece[] UnitPrefabs { get => unitPrefabs; set => unitPrefabs = value; }


        public void SpawnObject(int unitIndex)
        {
            if (!hasAuthority) return;
            CmdSpawnUnit(unitIndex);
        }
        
        [Command]
        private void CmdSpawnUnit(int unitIndex)
        {
            NetworkPlayerChess player = connectionToClient.identity.GetComponent<NetworkPlayerChess>();
        }

    }
}