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
        [SerializeField] private int trys = 50;

        public override void Awake()
        {
            base.Awake();
            
            OnTurn += StartMove;
        }

        private void StartMove()
        {
            List<Moves> myPossibleMoves = AllLegalMoves();
            Moves move = HighestValueMove(myPossibleMoves);
            List<Moves> opponentsPossibleMoves = otherPlayer.AllLegalMoves();
            bool okToMove = false;
            int giveUpCount = 0;
            while (!okToMove && giveUpCount < trys)
            {
                trys++;
                okToMove = true;
                foreach (Moves m in opponentsPossibleMoves)
                {
                    if (m.MoveResultPos == move.MoveResultPos)
                    {
                        okToMove = false;
                        Debug.Log("Move conflict");
                        move = myPossibleMoves[Random.Range(0, myPossibleMoves.Count)];
                        break;
                    }
                }
            }
            StartCoroutine(move.Piece.AIMove(move));
        }
    }
}