using System;
using System.Collections.Generic;
using Chess.Enums;
using Chess.Interface;
using Chess.Movement;
using Chess.Pieces;
using Unity.VisualScripting;
using UnityEngine;

namespace Chess.Control
{
    public class Controller : MonoBehaviour, IController
    {
        internal Team _team = Team.Black;
        [SerializeField] bool _isActive = false;
        public event Action<Controller> OnMoved;
        internal event Action OnTurn;
        internal List<ChessPiece> pieces = new List<ChessPiece>();
        public Team GetTeam()
        {
            return _team;
        }

        public bool IsActive()
        {
            return _isActive;
        }

        public void SetActive()
        {
            _isActive = true;
            OnTurn?.Invoke();
            // Debug.Log($"Set Active {_team}");
        }
        public List<Moves> AllPossibleMoves()
        {
            List<Moves> possibleMoves = new List<Moves>();
            foreach (ChessPiece piece in pieces)
            {
                possibleMoves.AddRange(piece.GetPossibleMoves(this));
            }

            return possibleMoves;
        }

        public void PiecesCallToController(ChessPiece piece)
        {
            pieces.Add(piece);
        }

        public void SetTeam(Team team)
        {
            _team = team;
        }

        public void MoveMade()
        {
            // Debug.Log($"Move Made for {_team}");
            OnMoved?.Invoke(this);
            _isActive = false;
        }
    }
}