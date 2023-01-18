using Chess.Pieces;

namespace Chess.Movement
{
    public class Move
    {
        public Piece Piece { get; set; }
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int DestY { get; set; }
        public int DestX { get; set; }
        public bool IsPromotion { get; set; }
    }
}