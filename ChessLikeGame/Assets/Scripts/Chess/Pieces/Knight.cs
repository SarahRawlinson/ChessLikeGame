using Chess.Enums;
using Chess.Movement;
using UnityEngine;

namespace Chess.Pieces
{
    public class Knight: ChessPiece
    {
        protected override void Awake()
        {
            base.Awake();
            NameType = "Knight";
            Key = "N";
            // WorkOutMoves();
        }

        public override void WorkOutMoves()
        {
            MovesGroupList.Add(new MoveGroup(MoveTypes.L, Overtake.Both, true));
            MovesList = GetMoves(1);
        }

        public override bool GetConditionsMet(MoveTypes move, int step, Overtake overtake, bool jump)
        {
            // Debug.Log("Knight");
            if (move == MoveTypes.L)
            {
                return true;
            }
            return false;
        }
        public override void WorkoutIfMoved()
        {
            if (pos.x != 1 && pos.x != 6)
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
                Debug.Log("Knight Has Moved");
            }
        }
    }
}