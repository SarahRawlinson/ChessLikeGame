using System.Collections.Generic;
using Multiplayer.Models.Movement;

namespace Multiplayer.Models.Rules
{
    public class KnightMoves : IPieceMovement
    {
        public List<MoveToValidate> possibleMoves()
        {
            List<MoveToValidate> myMoves = new List<MoveToValidate>();
            List<MoveValidationTypes> L = new List<MoveValidationTypes>();
            myMoves.Add(new MoveToValidate(MoveTypes.L, L,1));
            return myMoves;
        }
    }
}