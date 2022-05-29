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
        private Controller otherPlayer;
        private void Awake()
        {
            foreach (Controller con in FindObjectsOfType<Controller>())
            {
                if (con != this)
                {
                    otherPlayer = con;
                    break;
                }
            }
            OnTurn += StartMove;
        }

        private void StartMove()
        {
            List<Moves> myPossibleMoves = AllPossibleMoves();
            
            Moves move = myPossibleMoves[Random.Range(0, myPossibleMoves.Count)];
            myPossibleMoves.Sort(Comparison);
            if (myPossibleMoves[^1].moveValue > 0)
            {
                move = myPossibleMoves[^1];
                
                
            }
            List<Moves> opponentsPossibleMoves = otherPlayer.AllPossibleMoves();
            foreach (Moves m in opponentsPossibleMoves)
            {
                if (m.MoveResultPos == myPossibleMoves[^1].MoveResultPos)
                {
                    Debug.Log("Move conflict");
                    move = myPossibleMoves[Random.Range(0, myPossibleMoves.Count)];
                }
                else
                {
                    // Debug.Log("Move OK");
                }
            }
            StartCoroutine(move.Piece.AIMove(move));
        }


        private int Comparison(Moves x, Moves y)
        {
            return (int)(x.moveValue - y.moveValue);
        }
    }
}