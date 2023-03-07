using System;

namespace Multiplayer.Models
{
    public class MultiPiece
    {
        public MultiPiece(int x, int y, TeamColor color, bool hasMoved)
        {
            X = x;
            Y = y;
            Colour = color;
            _hasMoved = hasMoved;
        }

        public MultiPiece()
        {
            _hasMoved = false;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public TeamColor Colour { get; set; }

        private bool _hasMoved;
        public ChessPieceTypes type;
    } 
}
