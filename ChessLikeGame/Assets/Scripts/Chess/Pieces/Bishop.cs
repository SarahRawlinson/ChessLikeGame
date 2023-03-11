using Chess.Enums;
using Chess.Movement;
using Multiplayer.Models;
using Multiplayer.Models.Movement;
using UnityEngine;

namespace Chess.Pieces
{
    public class Bishop: ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            NameType = "Bishop";
            Key = "B";
            // WorkOutMoves();
        }

        public override void WorkOutMoves()
        {
            MovesGroupList.Add(new MoveGroup(MoveTypes.DiagonalUpLeft, Overtake.Both, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.DiagonalUpRight, Overtake.Both, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.DiagonalDownLeft, Overtake.Both, false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.DiagonalDownRight, Overtake.Both, false));
            WorkOutMovesUnlimitedSteps();
        }


        public override bool GetConditionsMet(MoveTypes move, int step, Overtake overtake, bool jump)
        {
            // Debug.Log("Bishop");
            if (move is MoveTypes.DiagonalDownLeft 
                or MoveTypes.DiagonalDownRight 
                or MoveTypes.DiagonalUpLeft 
                or MoveTypes.DiagonalUpRight)
            {
                return true;
            }
            return false;
        }
        public override void WorkoutIfMoved()
        {
            if (pos.x != 2 && pos.x != 5)
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
                // Debug.Log("Bishop Has Moved");
            }
        }
    }
}