using System.Collections.Generic;
using Multiplayer.Models.Movement;

namespace Multiplayer.Models.Rules
{
    public class KingMoves : IPieceMovement
    {
        public List<MoveToValidate> possibleMoves()
        {
            List<MoveToValidate> myMoves = new List<MoveToValidate>();
            List<MoveValidationTypes> K = new List<MoveValidationTypes>() {MoveValidationTypes.CheckKingCantBeTaken};
            
            myMoves.Add(new MoveToValidate(MoveTypes.Forward, K, 1));
            myMoves.Add(new MoveToValidate(MoveTypes.Backward, K, 1));
            myMoves.Add(new MoveToValidate(MoveTypes.Left, K, 1));
            myMoves.Add(new MoveToValidate(MoveTypes.Right, K, 1));
            myMoves.Add(new MoveToValidate(MoveTypes.DiagonalDownLeft, K, 1));
            myMoves.Add(new MoveToValidate(MoveTypes.DiagonalDownRight, K, 1));
            myMoves.Add(new MoveToValidate(MoveTypes.DiagonalUpLeft, K, 1));
            myMoves.Add(new MoveToValidate(MoveTypes.DiagonalUpRight, K, 1));
            
            myMoves.Add(new MoveToValidate(
                MoveTypes.CastleKingSide, 
                new List<MoveValidationTypes>()
                {
                    MoveValidationTypes.CheckKingCantBeTaken, MoveValidationTypes.CheckHasNotMoved, MoveValidationTypes.CheckIsKingOrRook
                },
                1));
            myMoves.Add(new MoveToValidate(
                MoveTypes.CastleQueenSide, 
                new List<MoveValidationTypes>()
                {
                    MoveValidationTypes.CheckKingCantBeTaken, MoveValidationTypes.CheckHasNotMoved, MoveValidationTypes.CheckIsKingOrRook
                },
                1));

            return myMoves;
        }
    }
}