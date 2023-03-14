using System;

namespace Multiplayer.Models.BoardState
{
    [Serializable]
    public enum ChessPieceTypes
    {
        NONE,
        PAWN,
        BISHOP,
        KNIGHT,
        ROOK,
        QUEEN,
        KING 
    }
}