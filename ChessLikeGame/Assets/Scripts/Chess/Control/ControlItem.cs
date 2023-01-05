using System.Collections.Generic;
using Chess.Networking;
using Chess.Pieces;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Chess.Control
{
    public class ControlItem : NetworkBehaviour
    {
        [FormerlySerializedAs("nameOfIteam")] [SerializeField] string nameOfItem = "";
        [SerializeField] internal GameObject buildPrefab = null;
        [SerializeField] internal Sprite icon = null;
        [SerializeField] internal int id = -1;
        [SerializeField] internal int price = 100;
        [SyncVar] private int connectionID;
        public GameObject BuildPrefab { get => buildPrefab; }
        public Sprite Icon { get => icon; }
        public int ID { get => id; }
        public int Price { get => price; }
        public string NameOfItem
        {
            get => nameOfItem;
        }
        private string _description = "";

        public string GetDescription()
        {
            if (_description == "") _description = BuildDescription();
            return _description;
        }

        public override void OnStartServer()
        {
            connectionID = connectionToClient.connectionId;
        }

        private string BuildDescription()
        {
            return buildPrefab.name;
        }

        

        private string SpawnerDataString
        {
            get
            {
                string dataString = $"{nameOfItem}\n";
                if (TryGetComponent(out RTSUnitSpawner spawner))
                {
                    string units = "";
                    foreach (ChessPiece unit in (IEnumerable<ChessPiece>) spawner.UnitPrefabs) units += $"{unit.nameOfItem}, ";
                    if (units != "") units = units.Remove(units.Length - 2, 2);
                    dataString += $"Has Spawner, Units {units}\n";
                }

                return dataString;
            }
        }

        public NetworkPlayerChess GetPlayer()
        {
            foreach (NetworkPlayerChess player in FindObjectsOfType<NetworkPlayerChess>())
            {
                if (connectionID == player.connectionID)
                {
                    return player;
                }
            }
            return null;
        }
        
    }
}