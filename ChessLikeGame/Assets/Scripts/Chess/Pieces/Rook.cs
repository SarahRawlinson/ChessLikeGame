using Chess.Enums;
using Chess.Movement;
using UnityEngine;

namespace Chess.Pieces
{
    public class Rook: ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            NameType = "Rook";
            Key = "R";
            // WorkOutMoves();
        }

        public override void WorkOutMoves()
        {
            MovesGroupList.Add(new MoveGroup(MoveTypes.Forward, Overtake.Both,false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.Backward, Overtake.Both,false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.Left, Overtake.Both,false));
            MovesGroupList.Add(new MoveGroup(MoveTypes.Right, Overtake.Both,false));
            WorkOutMovesUnlimitedSteps();
        }

        public override bool GetConditionsMet(MoveTypes move, int step, Overtake overtake, bool canJump)
        {
            // Debug.Log("Rook");
            if (move is MoveTypes.Forward 
                or MoveTypes.Backward 
                or MoveTypes.Left 
                or MoveTypes.Right)
            {
                return true;
            }
            return false;
        }
        
        public override void WorkoutIfMoved()
        {
            if (pos.x != 0 && pos.x != 7)
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
                Debug.Log("Rook Has Moved");
            }
        }
    }
}