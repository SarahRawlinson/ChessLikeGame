using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketTest : MonoBehaviour
{
    private ClientWebSocket webSocket;

    // Start is called before the first frame update
    async void Start()
    {
        // Create a new WebSocket instance and connect to the server
        webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://localhost:8080/");
        await webSocket.ConnectAsync(serverUri, CancellationToken.None);

        // Send a message to the server
        string message = "Hello, server!";
        ArraySegment<byte> buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
        await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

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
            }
        }
    }

    // Close the WebSocket connection when the script is destroyed
    private async void OnDestroy()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Script destroyed", CancellationToken.None);
        }
    }
}