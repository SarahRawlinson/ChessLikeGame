using System;
using Chess.Board;
using Chess.Enums;
using Chess.Pieces;
using UnityEngine.Serialization;

namespace Chess.Movement
{
    [Serializable]
    public class Moves
    {
        public int Right;
        public int Forward;
        public Overtake Overtake;
        public MoveTypes MoveType;
        public int GroupIndex;
        public ChessPiece Piece;
        public ChessPiece SwapPiece;
        public (int x, int y) MoveResultPos;
        public float MoveValue = 0;
        public Moves(int x, int y, MoveGroup moveGroup, int index, ChessPiece piece)
        {
            Right = x;
            Forward = y;
            Overtake = moveGroup.Overtake;
            MoveType = moveGroup.Type;
            GroupIndex = index;
            Piece = piece;
        }
        public Moves(int x, int y, MoveGroup moveGroup, int index, ChessPiece piece, ChessPiece swapPiece)
        {
            Right = x;
            Forward = y;
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