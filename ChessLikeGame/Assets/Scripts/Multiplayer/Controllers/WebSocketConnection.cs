using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Multiplayer.Models.Connection;
using NetClient;
using UnityEngine;


namespace Multiplayer.Controllers
{
    public class WebSocketConnection : MonoBehaviour
    {
        private ClientWebSocket webSocket;

        private Client client = new Client();
       
        public static event Action<bool> onAuthenicate;
        public static event Action<bool> onHostGame;
        public static event Action<string>onHostsList;
        public static event Action<string>onUsersList;
        public User user { get; set; }
        private int clientID = -1;

        // Start is called before the first frame update
        public async void Connect(User userData)
        {
            await client.Connect();
            await client.Authenticate(userData.Username, userData.Password);
            
            
            
            // user = userData;
            // await ConnectToWebSocket();
            // StartCoroutine(nameof(Listen));
            // await AthenticateUser(user);
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
                        case "IDIS":
                            clientID = Int32.Parse(message[1]);
                            break;
                        case "ROOMLIST":
                            if (message.Length < 3)
                            {
                                break;
                            }
                            onHostsList?.Invoke(receivedMessage.Substring(message[0].Length + message[1].Length + 2));
                            break;
                        case "USERLIST":
                            if (message.Length < 3)
                            {
                                break;
                            }
                            onUsersList?.Invoke(receivedMessage.Substring(message[0].Length + message[1].Length + 2));
                            break;
                        case "RECIEVEMESSAGE":
                            Debug.Log(receivedMessage.Substring(message[0].Length + 1));
                            break;
                        case "ROOMJOINED":
                            Debug.Log($"joined room {message[1]}");
                            break;
                        case "ROOMCREATED":
                            Debug.Log($"room {message[1]} has been created");
                            break;
                        case "USERJOINED":
                            Debug.Log($"{message[1]} joined room");
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public void SendMessageToUser(string userName, string Message)
        {
            string msg = $"SENDMESGTOUSER:{userName}:{Message}";
            SendMessage(msg);
        }

        
        public void GetRoomList()
        {
            SendMessage("GETROOMLIST");
        }
        
        public void CreateNewRoom(int roomSize, bool isPublic)
        {
            SendMessage($"CREATEROOM:{roomSize}:{(isPublic?"PUBLIC":"PRIVATE")}");
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