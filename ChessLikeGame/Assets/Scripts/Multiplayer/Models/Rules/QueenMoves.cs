using System.Collections.Generic;
using Multiplayer.Models.Movement;

namespace Multiplayer.Models.Rules
{
    public class QueenMoves : IPieceMovement
    {
        public List<MoveToValidate> possibleMoves()
        {
            List<MoveToValidate> myMoves = new List<MoveToValidate>();
            List<MoveValidationTypes> Q = new List<MoveValidationTypes>() {MoveValidationTypes.CheckForClearPath};

            for (int i = 1; i < 8; i++)
            {
                myMoves.Add(new MoveToValidate(MoveTypes.Forward, Q, i));
                myMoves.Add(new MoveToValidate(MoveTypes.Backward, Q, i));
                myMoves.Add(new MoveToValidate(MoveTypes.Left, Q, i));
                myMoves.Add(new MoveToValidate(MoveTypes.Right, Q, i));
                myMoves.Add(new MoveToValidate(MoveTypes.DiagonalDownLeft, Q, i));
                myMoves.Add(new MoveToValidate(MoveTypes.DiagonalDownRight, Q, i));
                myMoves.Add(new MoveToValidate(MoveTypes.DiagonalUpLeft, Q, i));
                myMoves.Add(new MoveToValidate(MoveTypes.DiagonalUpRight, Q, i));
            }
            return myMoves;
        }
    }
}