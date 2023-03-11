using System;

namespace Multiplayer.Models.Movement
{
    [Serializable]
    //todo: remove CastleQueenSideWhite,CastleQueenSideBlack,CastleKingSideWhite,CastleKingSideBlack when replace the old code
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
        CastleKingSideBlack,
        CastleKingSide,
        CastleQueenSide
    }
}