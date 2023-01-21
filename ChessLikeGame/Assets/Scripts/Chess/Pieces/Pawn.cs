using System;
using System.Linq;
using Chess.Board;
using Chess.Control;
using Chess.Enums;
using Chess.Movement;
using Unity.VisualScripting;
using UnityEngine;

namespace Chess.Pieces
{
    public class Pawn : ChessPiece
    {
        private bool firstMove = true;
        private int endY;
        
        private void Start()
        {
            OnMove += WhenMove;
            NameType = "Pawn";
            Key = "P";
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

        void WhenMove(((int x, int y) from, (int x, int y) to) valueTuple)
        {
            OnMoveStatic?.Invoke();
            var fromY = valueTuple.from.y;
            var toY = valueTuple.to.y;
            if (firstMove && Mathf.Abs(fromY - toY) == 2)
            {
                ActivateEnPassant(valueTuple, fromY, toY);
                PieceController.OnMoved += DeactivateEnPassant;
            }
            firstMove = false;
            OnMove -= WhenMove;
        }

        private void ActivateEnPassant(((int x, int y) from, (int x, int y) to) valueTuple, int fromY, int toY)
        {
            enPassant = true;
            // not sure if this is the correct way around need to test
            EnPassantString =
                $"{Position.Number2String(valueTuple.from.x).ToLower()}{fromY + 1 + (fromY - toY > 0 ? 1 : -1)}";
        }

        private void DeactivateEnPassant(Controller obj)
        {
            enPassant = false;
            EnPassantString = "";
        }

        public override void SetPosition()
        {
            base.SetPosition();
            if (_startPosition.y == 0)
            {
                endY = _board.rows;
            }
            if (_startPosition.y == _board.rows)
            {
                endY = 0;
            }
        }

        public override void SpecialActions(int x, int y)
        {
            base.SpecialActions(x, y);
            if (y == endY)
            {
                SwapPiece(this, ReturnNewPiece());
            }
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

        public static event Action OnMoveStatic;
    } 
}

