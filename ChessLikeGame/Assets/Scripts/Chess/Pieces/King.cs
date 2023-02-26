using System;
using System.Collections.Generic;
using System.Net;
using Chess.Board;
using Chess.Control;
using Chess.Enums;
using Chess.Movement;
using UnityEngine;

namespace Chess.Pieces
{
    public class King: ChessPiece
    {
        public static event Action<Team> OnEnd;
        public bool canCastleKing;
        public bool canCastleQueen;
        public bool CanCastleKing { get => canCastleKing; set => CanCastle(ref canCastleKing, value); }
        public bool CanCastleQueen { get => canCastleQueen; set => CanCastle(ref canCastleQueen, value); }
        private bool inCheck = false;
        private Director _director;

        private void CanCastle(ref bool castle, bool value)
        {
            castle = value;
            // Debug.Log($"can castle called {value}");
        }
        protected override void Awake()
        {
            base.Awake();
            NameType = "King";
            Key = "K";
            // WorkOutMoves();
            OnTaken += End;
            _director = FindObjectOfType<Director>();
        }

        private void End()
        {
            OnEnd?.Invoke(team);
        }

        public override void WorkOutMoves()
        {
            MovesGroupList.Add(new MoveGroup(MoveTypes.Forward, Overtake.Both, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.Backward, Overtake.Both, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.Left, Overtake.Both, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.Right, Overtake.Both, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.DiagonalUpLeft, Overtake.Both, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.DiagonalUpRight, Overtake.Both, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.DiagonalDownLeft, Overtake.Both, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.DiagonalDownRight, Overtake.Both, false));
            if (CanCastleKing)
            {
                // Debug.Log($"{team.ToString()} can king castle");
                MovesGroupList.Add(
                    new MoveGroup(
                        team==Team.Black ? MoveTypes.CastleKingSideBlack : MoveTypes.CastleKingSideWhite, 
                        Overtake.Both, 
                        true
                    )
                    );
            }
            if (CanCastleQueen)
            {
                // Debug.Log($"{team.ToString()} can queen castle");
                MovesGroupList.Add(
                    new MoveGroup(
                        team==Team.Black ? MoveTypes.CastleQueenSideBlack : MoveTypes.CastleQueenSideWhite, 
                        Overtake.Both, 
                        true
                    )
                );
            }
            MovesList = GetMoves(1);
        }

        internal override List<Moves> GetPossibleMoves(Controller controller)
        {
            List<Moves> possibleMoves = base.GetPossibleMoves(controller);
            List<Moves> actualMoves = new List<Moves>();
            foreach (Moves move in possibleMoves)
            {
                if (!_director.IsCheck(team, move.MoveResultPos))
                {
                    actualMoves.Add(move);
                }
            }
            return actualMoves;
        }


        public override bool GetConditionsMet(MoveTypes move, int step, Overtake overtake, bool jump)
        {
            // Debug.Log("King");
            if (move is MoveTypes.Forward 
                or MoveTypes.Backward 
                or MoveTypes.Left 
                or MoveTypes.Right 
                or MoveTypes.DiagonalDownLeft 
                or MoveTypes.DiagonalDownRight 
                or MoveTypes.DiagonalUpLeft 
                or MoveTypes.DiagonalUpRight
                && step == 1 ||
                move is MoveTypes.CastleKingSideBlack
                or MoveTypes.CastleKingSideWhite
                or MoveTypes.CastleQueenSideBlack
                or MoveTypes.CastleQueenSideWhite)
            {
                return true;
            }
            return false;
        }
        
        public override void WorkoutIfMoved()
        {
            if (pos.x != 4)
            {
                HasMoved = true;
            }
            else if (team == Team.Black)
            {
                HasMoved = pos.y != 7;
            }
            else if (team == Team.White)
            {
                HasMoved = pos.y != 0;
            }

            if (HasMoved)
            {
                // Debug.Log("King Has Moved");
            }
        }
        
        public override void SpecialActions(int x, int y, Position positionFrom, Position positionTo)
        {
            base.SpecialActions(x, y, positionFrom, positionTo);
            // if (positionFrom.hasReplacementPiece)
            // {
            //     ChessPiece piece = positionFrom.replacementPiece;
            //     piece.SetActive();
            //     piece.Move(positionFrom);
            // }
        }

        public void SetInCheck(bool on)
        {
            inCheck = on;
        }
    }
}