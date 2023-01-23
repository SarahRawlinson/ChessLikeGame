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
        
        protected override void Awake()
        {
            base.Awake();
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

        public override void WorkoutIfMoved()
        {
            if (team == Team.Black)
            {
                HasMoved = pos.y != 6;
            }
            else if (team == Team.White)
            {
                HasMoved = pos.y != 1;
            }

            if (HasMoved)
            {
                Debug.Log("Pawn Has Moved");
            }
        }

        void WhenMove(((int x, int y) from, (int x, int y) to) valueTuple)
        {
            OnMoveStatic?.Invoke();
            var fromY = valueTuple.from.y;
            var toY = valueTuple.to.y;
            if (firstMove && Mathf.Abs(fromY - toY) == 2)
            {
                ActivateEnPassant(valueTuple, fromY, toY);
                Director.OnMoveMade += DeactivateEnPassant;
            }
            firstMove = false;
            OnMove -= WhenMove;
        }

        private void DeactivateEnPassant()
        {
            // Debug.Log($"En Passant Deactivated {EnPassantString}");
            enPassant = false;
            EnPassantString = "";
        }

        private void ActivateEnPassant(((int x, int y) from, (int x, int y) to) valueTuple, int fromY, int toY)
        {
            enPassant = true;
            // not sure if this is the correct way around need to test
            EnPassantString =
                $"{Position.Number2String(valueTuple.from.x).ToLower()}{fromY + 1 + (fromY - toY > 0 ? -1 : 1)}";
            // Debug.Log($"En Passant Active {EnPassantString}");
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

        public override void SpecialActions(int x, int y, Position positionFrom, Position positionTo)
        {
            base.SpecialActions(x, y, positionFrom, positionTo);
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

