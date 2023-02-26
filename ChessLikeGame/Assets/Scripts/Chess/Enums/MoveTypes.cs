using System;

namespace Chess.Enums
{
    [Serializable]
    public enum MoveTypes
    {
        Forward,
        Backward,
        Left,
        Right,
        DiagonalUpRight,
        DiagonalUpLeft,
        DiagonalDownRight,
        DiagonalDownLeft,
        L,
        CastleQueenSideWhite,
        CastleQueenSideBlack,
        CastleKingSideWhite,
        CastleKingSideBlack
    }
}