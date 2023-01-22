using Chess.Enums;
using Chess.Movement;
using UnityEngine;

namespace Chess.Pieces
{
    public class Knight: ChessPiece
    {
        private void Start()
        {
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
    }
}