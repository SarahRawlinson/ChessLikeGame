using Chess.Board;
using Chess.Enums;
using Chess.Pieces;

namespace Chess.Movement
{
    public class Moves
    {
        public readonly int Forward;
        public readonly int Right;
        public readonly Overtake Overtake;
        public readonly MoveTypes MoveType;
        public readonly int GroupIndex;
        public readonly ChessPiece Piece;
        public readonly ChessPiece SwapPiece;
        public (int x, int y) MoveResultPos;
        public float MoveValue = 0;
        public Moves(int x, int y, MoveGroup moveGroup, int index, ChessPiece piece)
        {
            Forward = x;
            Right = y;
            Overtake = moveGroup.Overtake;
            MoveType = moveGroup.Type;
            GroupIndex = index;
            Piece = piece;
        }
        public Moves(int x, int y, MoveGroup moveGroup, int index, ChessPiece piece, ChessPiece swapPiece)
        {
            Forward = x;
            Right = y;
            Overtake = moveGroup.Overtake;
            MoveType = moveGroup.Type;
            GroupIndex = index;
            Piece = piece;
            SwapPiece = swapPiece;
            IsCastle = swapPiece!=null;
        }

        public bool IsCastle { get; }
    }
}