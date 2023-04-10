using System;
using System.Collections.Generic;
using LibObjects;
using Multiplayer.Controllers;
using Multiplayer.View.DisplayData;
using Multiplayer.View.UI;
using TMPro;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class LoadChatLogUI : MonoBehaviour
    {
        [SerializeField] private DisplayLogData logPrefab;
        [SerializeField] private ScrollContentUI _scrollContentUI;
        private List<DisplayLogData> _logs = new List<DisplayLogData>();

        private void Awake()
        {
            WebSocketConnection.onMessageRecieved += LogDataMessageRecieved;
            WebSocketConnection.onAuthenicate += LogDataAuthenticate;
            WebSocketConnection.onUserLoggedIn += LogDataUserLoggedIn;
            WebSocketConnection.onUserDisconnect += LogDataUserDisconnect;
            WebSocketConnection.onHostChat += LogDataHostedChat;
            WebSocketConnection.onJoinedChat += LogDataJoinedChat;
            WebSocketConnection.onLeftGame += LogDataLeftGame;
            WebSocketConnection.onUserJoinedChatRoom += LogDataUserJoinedChatRoom;
            WebSocketConnection.onUserLeftChatRoom += LogDataUserLeftChatRoom;
            WebSocketConnection.onRecievedYouWhereRemovedFromTheRoom += LogDataReceivedYouWhereRemovedFromTheRoom;
            WebSocketConnection.onRecievedYouHaveBeenBannedFromRoom += LogDataReceivedYouHaveBeenBannedFromRoom;
            WebSocketConnection.onRecievedYouAreNoLongerBannedFromRoom += LogDataReceivedYouAreNoLongerBannedFromRoom;
            WebSocketConnection.onRecievedYouHaveBeenApprovedForRoom += LogDataReceivedYouHaveBeenApprovedForRoom;
            WebSocketConnection.onRecievedYouAreNoLongerApprovedForRoom += LogDataReceivedYouAreNoLongerApprovedForRoom;
            WebSocketConnection.onChatRoomMessageRecieved += LogDataChatRoomMessageReceived;
            WebSocketConnection.onChatRoomPrivateMessageRecieved += LogDataChatRoomPrivateMessageReceived;
            WebSocketConnection.onReceivedCommunicationToAll += LogDataReceivedCommunicationToAll;
            WebSocketConnection.onRoomDestroyed += LogDataRoomDestroyed;
            WebSocketConnection.onLoggedOff += LogDataLoggedOff;
            WebSocketConnection.onOnErrorMessageReceived += ErrorMessageReceived;
        }

        private void LogDataLoggedOff()
        {
            LogData($"Logged Off");
        }

        private void ErrorMessageReceived((CommunicationTypeEnum comEnum, string message) obj)
        {
            LogData($"Error message received {obj.comEnum.ToString()}: {obj.message}");
        }

        private void LogDataReceivedCommunicationToAll((User user, string messageSent) obj)
        {
            LogData($"Received message to all from {obj.user.GetUserName()}: {obj.messageSent}");
        }

        private void LogDataRoomDestroyed(Room obj)
        {
            LogData($"Room Destroyed {obj.GetRoomName()}");
        }

        private void LogDataChatRoomMessageReceived((Room room, User user, string Message) obj)
        {
            LogData($"Received Chat Room Message from {obj.user.GetUserName()}: {obj.Message}");
        }
        private void LogDataChatRoomPrivateMessageReceived((Room room, User user, string Message) obj)
        {
            LogData($"Received Private Chat Room Message from {obj.user.GetUserName()}: {obj.Message}");
        }

        private void LogDataReceivedYouAreNoLongerApprovedForRoom(Room obj)
        {
            LogData($"Removed from {obj.GetRoomName()} approved list");
        }

        private void LogDataReceivedYouHaveBeenApprovedForRoom(Room obj)
        {
            LogData($"Approved for {obj.GetRoomName()}");
        }

        private void LogDataReceivedYouAreNoLongerBannedFromRoom(Room obj)
        {
            LogData($"No longer banned from {obj.GetRoomName()}");
        }

        private void LogDataReceivedYouHaveBeenBannedFromRoom(Room obj)
        {
            LogData($"Banned from {obj.GetRoomName()}");
        }

        private void LogDataReceivedYouWhereRemovedFromTheRoom(Room obj)
        {
            LogData($"Kicked from {obj.GetRoomName()}");
        }

        private void LogDataUserLeftChatRoom((User user, Guid roomGuid) obj)
        {
            LogData($"{obj.user} left room guid: {obj.roomGuid.ToString()}");
        }

        private void LogDataUserJoinedChatRoom((User user, Guid roomGuid) obj)
        {
            LogData($"{obj.user} joined room guid: {obj.roomGuid.ToString()}");
        }

        private void LogDataJoinedChat(Room obj)
        {
            LogData($"Joined Room {obj.GetRoomName()}");
        }

        private void LogDataLeftGame(Room obj)
        {
            LogData($"Left Room {obj.GetRoomName()}");
        }

        private void LogDataHostedChat(Room obj)
        {
            LogData($"Created Room {obj.GetRoomName()}");
        }

        private void LogDataUserDisconnect(User obj)
        {
            LogData($"{obj.GetUserName()} Disconnected");
        }

        private void LogDataUserLoggedIn(User obj)
        {
            LogData($"{obj.GetUserName()} Online");
        }

        private void LogDataAuthenticate(bool obj)
        {
            if (obj)
            {
                LogData($"Logged in");
            }
            else
            {
                LogData($"Failed authentication");
            }
        }

        private void LogDataMessageRecieved((User user, string message) obj)
        {
            LogData($"Received message from {obj.user.GetUserName()}: {obj.message}");
        }


        private void LogData(string obj)
        {
            GameObject logObject = _scrollContentUI.AddContent(logPrefab.gameObject);
            DisplayLogData data = logObject.GetComponent<DisplayLogData>();
            string time = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            if (obj.Length > 40)
            {
                obj = $"{obj.Substring(0, 20)}...";
            }
            data.SetText($"{time}: {obj}");
        }
    }
}