using System;
using System.Collections.Generic;
using Chess.Enums;
using Chess.Interface;
using Chess.Movement;
using Chess.Pieces;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Chess.Control
{
    public class AI : Controller
    {
        private void Awake()
        {
            OnTurn += StartMove;
        }

        private void StartMove()
        {
            List<Moves> possibleMoves = new List<Moves>();
            foreach (ChessPiece piece in pieces)
            {
                possibleMoves.AddRange(piece.GetPossibleMoves());
            }
            Moves move = possibleMoves[Random.Range(0, possibleMoves.Count)];
            StartCoroutine(move.Piece.AIMove(move));
        }
    }
}