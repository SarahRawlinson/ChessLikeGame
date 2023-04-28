namespace Multiplayer.Models.BoardState
{
    public class MultiPiece
    {
        // private int x;
        // private int y;
        //
        // public int X => x;
        // public int Y => y;

        public TeamColor Colour { get; set; }
        public string key;
        

        private bool _hasMoved;
        private ChessPieceTypes type;
        
        public MultiPiece(TeamColor color, bool hasMoved)
        {
            // x = xa;
            // y = ya;
            Colour = color;
            _hasMoved = hasMoved;
        }

        public MultiPiece()
        {
            // x = xa;
            // y = ya;
            _hasMoved = false;
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

        // public void SetXY(int xa, int ya)
        // {
        //     x = xa;
        //     y = ya;
        // }
    } 
}
