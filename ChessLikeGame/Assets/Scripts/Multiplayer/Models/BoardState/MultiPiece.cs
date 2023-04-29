namespace Multiplayer.Models.BoardState
{
    public class MultiPiece
    {
        public TeamColor Colour { get; set; }
        public string key;
        private bool _hasMoved;
        private ChessPieceTypes type;
        
        public MultiPiece(TeamColor color, bool hasMoved)
        {
            Colour = color;
            _hasMoved = hasMoved;
        }

        public MultiPiece()
        {
            _hasMoved = false;
            Colour = TeamColor.Empty;
            type = ChessPieceTypes.NONE;
        }

        public void SetType(ChessPieceTypes pieceType)
        {
            type = pieceType;
        }
        
        public void SetKey( string charKey)
        {
            key = charKey;
        }

        public new ChessPieceTypes GetPieceType()
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

        public void SetMoved(bool value)
        {
            _hasMoved = value;
        }

        public override string ToString()
        {
            return $"Piece Color:{Colour} / Type:{type} / Key:{key} " ;
        }
    } 
}
