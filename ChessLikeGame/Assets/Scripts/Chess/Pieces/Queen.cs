using System.Linq;
using Chess.Enums;
using Chess.Movement;
using UnityEngine;

namespace Chess.Pieces
{
    public class Queen: ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            NameType = "Queen";
            Key = "Q";
            // WorkOutMoves();
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
            WorkOutMovesUnlimitedSteps();
        }


        public override bool GetConditionsMet(MoveTypes move, int step, Overtake overtake, bool jump)
        {
            // Debug.Log("Queen");
            if (move is MoveTypes.Forward 
                or MoveTypes.Backward 
                or MoveTypes.Left 
                or MoveTypes.Right 
                or MoveTypes.DiagonalDownLeft 
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
            if (pos.x != 3)
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
                // Debug.Log("Queen Has Moved");
            }
        }
    }
}