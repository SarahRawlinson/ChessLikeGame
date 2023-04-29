using System;
using System.Collections.Generic;
using Multiplayer.Models.Movement;

namespace Multiplayer.Models.Rules
{
    public class PawnMoves : IPieceMovement
    {
        public List<MoveToValidate> possibleMoves()
        {
            List<MoveToValidate> myMoves = new List<MoveToValidate>();
            List<MoveValidationTypes> diagonal = new List<MoveValidationTypes>() {MoveValidationTypes.CheckOccupied};
            List<MoveValidationTypes> forward = new List<MoveValidationTypes>() {MoveValidationTypes.CheckEmpty, MoveValidationTypes.CheckForClearPath};
            List<MoveValidationTypes> forwardIfNotMoved = new List<MoveValidationTypes>() {MoveValidationTypes.CheckEmpty, MoveValidationTypes.CheckHasNotMoved, MoveValidationTypes.CheckForClearPath};
            myMoves.Add(new MoveToValidate(MoveTypes.Forward, forward,1));
            myMoves.Add(new MoveToValidate(MoveTypes.Forward, forwardIfNotMoved,2));
            myMoves.Add(new MoveToValidate(MoveTypes.DiagonalUpLeft, diagonal,1));
            myMoves.Add(new MoveToValidate(MoveTypes.DiagonalUpRight, diagonal,1));
            return myMoves;
        }
    }
}