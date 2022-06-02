using System;
using System.Collections;
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
        [SerializeField] private int trys = 50;

        public override void Awake()
        {
            base.Awake();
            
            OnTurn += StartMove;
        }

        IEnumerator Move()
        {
            yield return new WaitForSeconds(2);
            List<Moves> myPossibleMoves = AllLegalMoves(this);
            List<Moves> opponentsPossibleMovesList = otherPlayer.AllLegalMoves(this);
            Moves move = HighestValueMove(myPossibleMoves, opponentsPossibleMovesList);
            StartCoroutine(move.Piece.AIMove(move));
        }

        private void StartMove()
        {
            StartCoroutine(Move());
        }
    }
}