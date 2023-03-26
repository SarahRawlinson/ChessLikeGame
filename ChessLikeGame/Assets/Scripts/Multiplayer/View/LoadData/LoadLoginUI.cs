using Multiplayer.Controllers;
using Multiplayer.Models.Connection;
using TMPro;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class LoadLoginUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField username;
        [SerializeField] private TMP_InputField password;
        [SerializeField] private TMP_Text errorMessage;
        [SerializeField] private GameObject activateGameObject;

        private void Start()
        {
            WebSocketConnection.onAuthenicate += LoginSuccess;
        }

        public void LoginSuccess(bool auth)
        {
            if (auth)
            {
                errorMessage.text = "";
                HideDisplay();
            }
            else
            {
                errorMessage.text = "Authentication Failed";
            }
        }

        public void Login()
        {
            WebSocketConnection socket = FindObjectOfType<WebSocketConnection>();
            socket.Connect(new User(username.text, password.text));
        }

        public void ShowDisplay()
        {
            activateGameObject.SetActive(true);
        }

        public void HideDisplay()
        {
            activateGameObject.SetActive(false);
        }
    }
}
