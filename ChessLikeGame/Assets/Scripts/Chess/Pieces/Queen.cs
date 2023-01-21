using System.Linq;
using Chess.Enums;
using Chess.Movement;
using UnityEngine;

namespace Chess.Pieces
{
    public class Queen: ChessPiece
    {
        private void Start()
        {
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
    }
}