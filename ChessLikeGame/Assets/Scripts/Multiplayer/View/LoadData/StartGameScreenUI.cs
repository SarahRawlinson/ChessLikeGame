﻿using System;
using System.Collections.Generic;
using LibObjects;
using Multiplayer.Controllers;
using Multiplayer.Models;
using Unity.VisualScripting;
using UnityEngine;

namespace Multiplayer.View.LoadData
{
    public class StartGameScreenUI : MonoBehaviour
    {
        [SerializeField] private GameObject showOnStart;
        [SerializeField] private GameObject hostOptions;
        public const string StartGameString = "Start Chess Game";
        private Room gameRoom;
        private WebSocketConnection _connection;
        private List<User> users;
        private User opponent;
        private User host;
        public static event Action<(Room gameRoom, User thisPlayer, User opponent, User host)> onGameStarted;

        public void Awake()
        {
            WebSocketConnection.onJoinedGame += ShowStartGameScreen;
            WebSocketConnection.onLeftGame += HideStartGameScreen;
            WebSocketConnection.onGameRoomMessageRecieved += CheckGameStarted;
            WebSocketConnection.onReceivedUsersListInRoom += UpdateUsers;
            _connection = FindObjectOfType<WebSocketConnection>();
        }

        private void UpdateUsers((Room room, List<User> users) obj)
        {
            foreach (var user in obj.users)
            {
                if (user.GetUserName() != _connection.GetClientUser().GetUserName())
                {
                    opponent = user;
                }

                if (user.GetUserName() == obj.room.GetCreator())
                {
                    host = user;
                }
            }
            users = obj.users;
        }

        private void CheckGameStarted((Room room, User user, string message) obj)
        {
            if (obj.room.GetGuid() != gameRoom.GetGuid())
            {
                return;
            }
            if (obj.room.GetCreator() != obj.user.GetUserName())
            {
                return;
            }
            
            if (obj.message == StartGameString)
            {
                Debug.Log("Start Game!");
                showOnStart.SetActive(false);
                onGameStarted?.Invoke((gameRoom, _connection.GetClientUser(), opponent, host));
            }
        }

        private void HideStartGameScreen(Room obj)
        {
            gameRoom = null;
            showOnStart.SetActive(false);
        }

        private void OnDestroy()
        {
            WebSocketConnection.onJoinedGame -= ShowStartGameScreen;
            WebSocketConnection.onLeftGame -= HideStartGameScreen;
            WebSocketConnection.onChatRoomMessageRecieved -= CheckGameStarted;
            WebSocketConnection.onReceivedUsersListInRoom -= UpdateUsers;
        }

        private void ShowStartGameScreen(Room obj)
        {
            _connection.AskForUsers(obj);
            hostOptions.SetActive(_connection.GetClientUser().GetUserName() == obj.GetCreator());
            gameRoom = obj;
            showOnStart.SetActive(true);
        }
        

        public void StartGame()
        {
            if (_connection.GetClientUser().GetUserName() == gameRoom.GetCreator())
            {
                _connection.SendMessageToRoom(gameRoom, StartGameString);
            }
        }
        
        public void ExitGame()
        {
            _connection.SendMessageToRoom(gameRoom, "Game Destroyed");
            _connection.LeaveRoom(gameRoom);
            showOnStart.SetActive(false);
        }
    }
}