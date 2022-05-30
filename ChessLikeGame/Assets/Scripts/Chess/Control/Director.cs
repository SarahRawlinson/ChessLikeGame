using System;
using Chess.Enums;
using UnityEngine;

namespace Chess.Control
{
    public class Director: MonoBehaviour
    {
        [SerializeField] private Controller player1;
        [SerializeField] private Controller player2;
        public event Action OnStart;
        private Controller _activeController;
        public Team team;

        private void Update()
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void NewGame()
        {
            StartGame();
        }

        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            player1.SetTeam(Team.Black);
            player1.OnMoved += MoveMade;
            player2.SetTeam(Team.White);
            OnStart?.Invoke();
            player1.SetActive();
        }

        private void MoveMade(Controller controller)
        {
            if (player1._team == controller._team)
            {
                player1.OnMoved -= MoveMade;
                player2.OnMoved += MoveMade;
                _activeController = player2;
                player2.SetActive();
                Debug.Log($"Active controller is now {player2}");
            }
            else
            {
                player2.OnMoved -= MoveMade;
                player1.OnMoved += MoveMade;
                _activeController = player1;
                player1.SetActive();
                Debug.Log($"Active controller is now {player1}");
            }
            team = _activeController._team;
            
            
        }
    }
}