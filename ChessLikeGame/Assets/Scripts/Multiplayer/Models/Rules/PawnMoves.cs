using System;
using System.Collections.Generic;

namespace Multiplayer.Models.Rules
{
    public class PawnMoves : IPieceMovement
    {
        public List<Move> possibleMoves(int startIndex, MultiPiece piece)
        {
            List<Move> tmpList = new List<Move>();
            
            //Forward 1 and 2 Squares
            tmpList.Add(new Move(Rules.incrementX(startIndex,1)));
            if(!piece.HasMoved())
                tmpList.Add(new Move(Rules.incrementX(startIndex,2)));

            switch (piece.Colour)
            {
                case TeamColor.Black:
                    break;
                case TeamColor.White:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Diagonal Right
            Move diagRight = new Move();
            diagRight.StartPosition = Rules.incrementX(startIndex, 1);
            diagRight.StartPosition = Rules.incrementY(diagRight.StartPosition, 1);
            tmpList.Add(diagRight);
            
            //Diagonal Left
            Move diagLeft = new Move();
            diagRight.StartPosition = Rules.incrementX(startIndex, 1);
            diagRight.StartPosition = Rules.incrementY(diagRight.StartPosition, 1);
            tmpList.Add(diagLeft);
            

            return tmpList;
        }
    }
}