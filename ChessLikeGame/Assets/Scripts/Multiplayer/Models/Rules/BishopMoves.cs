using System.Collections.Generic;
using Multiplayer.Models.Movement;

namespace Multiplayer.Models.Rules
{
    public class BishopMoves : IPieceMovement
    {
        public List<MoveToValidate> possibleMoves()
        {
            List<MoveToValidate> myMoves = new List<MoveToValidate>();
            List<MoveValidationTypes> B = new List<MoveValidationTypes>() {MoveValidationTypes.CheckForClearPath};

            for (int i = 1; i < 8; i++)
            {
                myMoves.Add(new MoveToValidate(MoveTypes.DiagonalDownLeft, B, i));
                myMoves.Add(new MoveToValidate(MoveTypes.DiagonalDownRight, B, i));
                myMoves.Add(new MoveToValidate(MoveTypes.DiagonalUpLeft, B, i));
                myMoves.Add(new MoveToValidate(MoveTypes.DiagonalUpRight, B, i));
            }
            return myMoves;
        }
    }
}