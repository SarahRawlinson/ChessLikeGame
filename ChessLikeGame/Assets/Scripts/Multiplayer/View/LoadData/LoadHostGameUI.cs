using System;
using LibObjects;
using Multiplayer.Controllers;
using Multiplayer.View.UI;
using TMPro;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class LoadHostGameUI : MonoBehaviour
    {
        [SerializeField] private GameObject createOnStartPrefab;
        [SerializeField] private TMP_InputField _inputFieldRoomName;
        [SerializeField] private ToggleButton _button;
        private GameObject activeObject = null;
        // Start is called before the first frame update
        void Start()
        {
            WebSocketConnection.onHostGame += StartGame;
        }

        private void StartGame(Room obj)
        {
            if (activeObject != null || obj != null) return;
            activeObject = Instantiate(createOnStartPrefab);
        }

        public void AskToHostGame()
        {
            FindObjectOfType<WebSocketConnection>().CreateNewGameRoom(2,_button.IsOn(), _inputFieldRoomName.text);
        }

        private void EndGame()
        {
            if (activeObject == null) return;
            Destroy(activeObject);
            activeObject = null;
        }

    }
}
