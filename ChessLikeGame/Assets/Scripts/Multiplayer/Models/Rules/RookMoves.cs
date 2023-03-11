using System.Collections.Generic;
using Multiplayer.Models.Movement;

namespace Multiplayer.Models.Rules
{
    public class RookMoves : IPieceMovement
    {
        public List<MoveToValidate> possibleMoves()
        {
            List<MoveToValidate> myMoves = new List<MoveToValidate>();
            List<MoveValidationTypes> R = new List<MoveValidationTypes>() {MoveValidationTypes.CheckForClearPath};

            for (int i = 1; i < 8; i++)
            {
                myMoves.Add(new MoveToValidate(MoveTypes.Forward, R, i));
                myMoves.Add(new MoveToValidate(MoveTypes.Backward, R, i));
                myMoves.Add(new MoveToValidate(MoveTypes.Left, R, i));
                myMoves.Add(new MoveToValidate(MoveTypes.Right, R, i));
            }
            return myMoves;
        }
    }
}