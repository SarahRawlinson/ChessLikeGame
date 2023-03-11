using System.Collections.Generic;

namespace Multiplayer.Models.Rules
{
    public interface IPieceMovement
    {
        public List<Move> possibleMoves(int startIndex, MultiPiece piece);
        
    }
}