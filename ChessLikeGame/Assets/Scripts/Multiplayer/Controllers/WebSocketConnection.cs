using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Multiplayer.Models.Connection;
using UnityEngine;

namespace Multiplayer.Controllers
{
    public class WebSocketConnection : MonoBehaviour
    {
        private ClientWebSocket webSocket;
        public static event Action<bool> onAuthenicate;
        public static event Action<string>onUserList;
        public User user { get; set; }

        // Start is called before the first frame update
        public async void Connect(User userData)
        {
            user = userData;
            await ConnectToWebSocket();
            await AthenticateUser(user);
            string GetUserListMessage = "GETUSERLIST";
            await SendMessage(GetUserListMessage);
            StartCoroutine(nameof(Listen));
            
        }

        private async Task Listen()
        {
            // Start a receive loop to handle incoming messages from the server
            byte[] receiveBuffer = new byte[1024];
            ArraySegment<byte> receiveSegment = new ArraySegment<byte>(receiveBuffer);
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(receiveSegment, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                    Debug.Log("Received message: " + receivedMessage);
                    string[] message = receivedMessage.Split(":");
                    if (message.Length < 2) return;
                    switch (message[0])
                    {
                        case "AUTH":
                            onAuthenicate?.Invoke(message[1] == "OK");
                            if (message[1] != "OK")
                            {
                                await CloseSocket();
                            }
                            break;
                        case "USERLIST":
                            onUserList?.Invoke(receivedMessage.Substring(9));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private async Task AthenticateUser(User user)
        {
            // Send a message to the server
            string Authmessage = $"AUTHENTICATE:{user.Username}:{user.Password}";
            await SendMessage(Authmessage);
        }

        private async Task ConnectToWebSocket()
        {
            // Create a new WebSocket instance and connect to the server
            webSocket = new ClientWebSocket();
            Uri serverUri = new Uri("ws://localhost:8080/");
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
        }

        private async Task SendMessage(string message)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

        }

        // Close the WebSocket connection when the script is destroyed
        private async void OnDestroy()
        {
            await CloseSocket();
        }

        private async Task CloseSocket()
        {
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Script destroyed", CancellationToken.None);
            }
        }
    }
}