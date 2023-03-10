using System;

namespace Multiplayer.Models
{
    public class MultiPiece
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TeamColor Colour { get; set; }
        public string key;
        

        private bool _hasMoved;
        private ChessPieceTypes type;
        
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

        public void SetType(ChessPieceTypes pieceType)
        {
            type = pieceType;
            key = type.ToString()[0].ToString();
        }

        public new ChessPieceTypes GetType()
        {
            return type;
        }

        public string GetKey()
        {
            return key;
        }

        public bool HasMoved()
        {
            return _hasMoved;
        }
    } 
}
