using System;
using Chess.Board;
using Chess.Enums;
using Chess.Pieces;

namespace Chess.Movement
{
    [Serializable]
    public class Moves
    {
        public int Forward;
        public int Right;
        public Overtake Overtake;
        public MoveTypes MoveType;
        public int GroupIndex;
        public ChessPiece Piece;
        public ChessPiece SwapPiece;
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