using Chess.Enums;
using Chess.Movement;
using UnityEngine;

namespace Chess.Pieces
{
    public class Bishop: ChessPiece
    {
        private void Start()
        {
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
    }
}