using Chess.Enums;
using Chess.Movement;
using UnityEngine;

namespace Chess.Pieces
{
    public class Rook: ChessPiece
    {
        private void Start()
        {
            NameType = "Rook";
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
    }
}