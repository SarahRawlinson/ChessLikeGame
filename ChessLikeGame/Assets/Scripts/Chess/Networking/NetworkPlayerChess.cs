using System;
using System.Collections.Generic;
using Chess.Control;
using Chess.Pieces;
using Chess.UI;
using Mirror;
using TMPro;
using UnityEngine;

// ReSharper disable once InconsistentNaming
namespace Chess.Networking
{
    public partial class NetworkPlayerChess : NetworkBehaviour
    {
        [System.Serializable]
        class CheckName
        {
            [SerializeField] public int maxLengthOfName = 10;
            [SerializeField] public int minLengthOfName = 3;
            [SerializeField] public List<string> illigalNames;
            [SerializeField] public List<char> bandCharactersSymbles;
        }
        
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private LayerMask buildLayer;
        [SerializeField] private ControlItem[] buildItems = Array.Empty<ControlItem>();
        [SerializeField] private TMP_Text displayNameText;
        [SerializeField] private Renderer displayColourRenderer;
        [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
        [SerializeField] private string displayName = "None";
        [SyncVar(hook = nameof(HandleDisplayColourUpdated))]
        [SerializeField] private Color displayColor = Color.white;
        [SerializeField] private CheckName checkName;
        [SyncVar] public int connectionID;
    
        
        public Color DisplayColor { get => displayColor; }
        public string DisplayName { get => displayName; }
        public List<ChessPiece> MyUnits { get => _myUnits; }
        public List<ControlItem> MyControlItems { get => _myControlItems; }
        public Transform CameraTransform { get => cameraTransform;}
        public bool IsPartOwner { get => _isPartOwner; }
        public ControlItem[] BuildItems { get => buildItems;  }
        public event Action<Color> ClientOnTeamColourUpdated;
        public event Action<string> ClientOnTeamNameUpdated;
        public static event Action<bool> AuthorityOnPartOwnerStateUpdated;
        public static event Action ClientOnInfoUpdated;

        [SyncVar(hook = nameof(AuthorityHandlePartOwnerChange))]
        private bool _isPartOwner;
        private readonly List<ChessPiece> _myUnits = new List<ChessPiece>();
        private readonly List<ControlItem> _myControlItems = new List<ControlItem>();
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private ControlItem TryValidateBuild(int prefabID, ControlItem buildToPlace)
        {
            foreach (ControlItem buildItem in buildItems)
            {
                if (buildItem.ID == prefabID)
                {
                    //Debug.Log("Build ID found");
                    buildToPlace = buildItem;
                    break;
                }
            }
            return buildToPlace;
        }
        

        private bool InRange(Vector3 point, ControlItem item, float itemRange)
        {
            bool inRange = (point - item.transform.position).sqrMagnitude <= itemRange * itemRange;

            return inRange;
        }

        #region Server

    

        public override void OnStartServer()
        {
            connectionID = connectionToClient.connectionId;
            ChessPiece.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            ChessPiece.ServerOnUnitDespawned += ServerHandleUnitDespawned;
            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopServer()
        {
            ChessPiece.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            ChessPiece.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        }

        

        private void ServerHandleUnitSpawned(ChessPiece unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            _myControlItems.Add(unit);
            _myUnits.Add(unit);
        }

        private void ServerHandleUnitDespawned(ChessPiece unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            _myControlItems.Remove(unit);
            _myUnits.Remove(unit);
        }
        

        [Server]
        public void SetDisplayName(string newDisplayName)
        {
            displayName = newDisplayName;
        }

        [Server]
        public void SetDisplayColor(Color newDisplayColor)
        {
            displayColor = newDisplayColor;
        }

        [Server]
        public void SetPartyOwner(bool state)
        {
            _isPartOwner = state;
        }

        [Command]
        public void CmdStartGame()
        {
            if (!_isPartOwner) return;
            ((NetworkManagerChess)NetworkManager.singleton).StartGame();
        }

        [Command]
        // ReSharper disable once UnusedMember.Local
        private void CmdSetDisplayName(string newDisplayName)
        {
            if (!CheckNameIsValid(newDisplayName))
            {
                Debug.Log("Name is not allowed");
                return;
            }
            //RpcLogNewName(newDisplayName);
            SetDisplayName(newDisplayName);
        }

        [Command]
        // ReSharper disable once UnusedMember.Local
        private void CmdSetDisplayColor(Color newDisplayColour)
        {
            SetDisplayColor(newDisplayColour);
        }


        private bool CheckNameIsValid(string nameToCheck)
        {
            if (nameToCheck.Length > checkName.maxLengthOfName || nameToCheck.Length < checkName.minLengthOfName) return false;
            foreach (char c in nameToCheck.ToLower())
            {
                foreach (char i in checkName.bandCharactersSymbles)
                {
                    string lowerI = i.ToString();
                    char ch = lowerI[0];
                    if (ch == c) return false;
                }
            }
            foreach (string bannedName in checkName.illigalNames)
            {
                if (nameToCheck.ToLower().Contains(bannedName.ToLower())) return false;
            }
            Debug.Log("Name is OK");
            return true;
        }

        #endregion

        #region client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;
            ChessPiece.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            ChessPiece.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        }

        public override void OnStartClient()
        {
            if (NetworkServer.active) return;
            DontDestroyOnLoad(gameObject);
            ((NetworkManagerChess)NetworkManager.singleton).Players.Add(this);
        }

        public override void OnStopClient()
        {
            ClientOnInfoUpdated?.Invoke();
            if (!isClientOnly) return;
            ((NetworkManagerChess)NetworkManager.singleton).Players.Remove(this);
            if (!hasAuthority) return;
            ChessPiece.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            ChessPiece.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

        }

        // ReSharper disable once UnusedParameter.Local
        private void AuthorityHandlePartOwnerChange(bool oldState, bool newState)
        {
            if (!hasAuthority) return;
            AuthorityOnPartOwnerStateUpdated?.Invoke(newState);
        }

        private void AuthorityHandleUnitSpawned(ChessPiece unit)
        {
            _myControlItems.Add(unit);
            _myUnits.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(ChessPiece unit)
        {
            _myControlItems.Remove(unit);
            _myUnits.Remove(unit);
        }
        

        // ReSharper disable once UnusedParameter.Local
        private void HandleDisplayColourUpdated(Color oldColour, Color newColour)
        {
            ClientOnTeamColourUpdated?.Invoke(newColour);
            displayNameText.color = newColour;
            displayColourRenderer.material.SetColor(BaseColor, newColour);
            ClientOnInfoUpdated?.Invoke();
        }

        // ReSharper disable once UnusedParameter.Local
        private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
        {
            ClientOnInfoUpdated?.Invoke();
            ClientOnTeamNameUpdated?.Invoke(newDisplayName);
            displayNameText.text = newDisplayName;
        }
        

        #endregion
    }
}

