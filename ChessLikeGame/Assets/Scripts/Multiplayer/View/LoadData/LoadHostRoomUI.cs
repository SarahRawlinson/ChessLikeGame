using System;
using LibObjects;
using Multiplayer.Controllers;
using Multiplayer.View.UI;
using TMPro;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class LoadHostRoomUI : MonoBehaviour
    {
        [SerializeField] private GameObject hideOnClose;
        [SerializeField] private GameObject createOnStartPrefab;
        [SerializeField] private TMP_InputField _inputFieldRoomName;
        [SerializeField] private ToggleButton _button;
        [SerializeField] private bool isGameRoom = false;
        [SerializeField] private bool hideFormOnJoined = false;
        private GameObject activeObject = null;
        // Start is called before the first frame update
        void Start()
        {
            WebSocketConnection.onHostGame += StartGame;
            WebSocketConnection.onHostChat += StartChat;
            WebSocketConnection.onJoinedGame += JoinedGame;
            WebSocketConnection.onJoinedChat += JoinedChat;
        }

        private void JoinedChat(Room obj)
        {
            if (!isGameRoom) HideForm();
        }

        private void JoinedGame(Room obj)
        {
            if (isGameRoom)HideForm();
        }

        private void StartChat(Room obj)
        {
            
        }

        private void StartGame(Room obj)
        {
            if (activeObject != null || obj != null) return;
            activeObject = Instantiate(createOnStartPrefab);
        }

        public void Host()
        {
            if (isGameRoom)
            {
                AskToHostGame();
            }
            else
            {
                AskToHostChat();
            }
        }

        private void HideForm()
        {
            if (hideFormOnJoined)
            {
                hideOnClose.SetActive(false);
            }
        }

        private void AskToHostGame()
        {
            FindObjectOfType<WebSocketConnection>().CreateNewGameRoom(2, _button.IsOn(), _inputFieldRoomName.text);
        }

        public void AskToHostChat()
        {
            FindObjectOfType<WebSocketConnection>().CreateNewChatRoom(20,_button.IsOn(), _inputFieldRoomName.text);
        }

        private void EndGame()
        {
            if (activeObject == null) return;
            Destroy(activeObject);
            activeObject = null;
        }

    }
}
