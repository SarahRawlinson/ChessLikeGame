using System;
using Chess.Enums;

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
        public Moves(int x, int y, MoveGroup moveGroup, int index)
        {
            Forward = x;
            Right = y;
            Overtake = moveGroup.Overtake;
            CanJump = moveGroup.CanJump;
            MoveType = moveGroup.Type;
            groupIndex = index;
        }
    }
}