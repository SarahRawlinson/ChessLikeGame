using System;
using System.Linq;
using Chess.Enums;
using Chess.Movement;
using Unity.VisualScripting;
using UnityEngine;

namespace Chess.Pieces
{
    public class Pawn : ChessPiece
    {
        private bool firstMove = true;
        private void Start()
        {
            OnMove += WhenMove;
            NameType = "Pawn";
            // WorkOutMoves();
        }

        public override void WorkOutMoves()
        {
            MovesGroupList.Add(new MoveGroup(MoveTypes.Forward, Overtake.No, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.DiagonalUpLeft, Overtake.Yes, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.DiagonalUpRight, Overtake.Yes, false));
            MovesList = GetMoves(1);
            MovesList.AddRange(GetMoves(2));
        }

        void WhenMove()
        {
            firstMove = false;
            OnMove -= WhenMove;
        }

        public override bool GetConditionsMet(MoveTypes move, int step, Overtake overtake, bool jump)
        {
            // Debug.Log("Pawn");
            if (move == MoveTypes.Forward 
                && overtake == Overtake.No)
            {
                if (firstMove)
                {
                    return (step == 2 || step == 1);
                }
                return step == 1;
            }
            if ((move is MoveTypes.DiagonalUpLeft 
                or MoveTypes.DiagonalUpRight) 
                && overtake == Overtake.Yes)
            {
                return step == 1;
            }
            return false;
        }
    } 
}

