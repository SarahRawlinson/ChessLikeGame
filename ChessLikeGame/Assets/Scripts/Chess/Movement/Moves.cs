using System;
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
        public MoveTypes MoveType;
        public bool CanJump;
        public int groupIndex;
        public ChessPiece Piece;
        public Position MoveResultPos;
        public float moveValue = 0;
        public ChessPiece PieceTaken;
        public Moves(int x, int y, MoveGroup moveGroup, int index, ChessPiece piece)
        {
            Forward = x;
            Right = y;
            Overtake = moveGroup.Overtake;
            CanJump = moveGroup.CanJump;
            MoveType = moveGroup.Type;
            groupIndex = index;
            Piece = piece;
        }
    }
}