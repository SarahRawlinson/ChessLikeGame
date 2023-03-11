using System;
using Chess.Enums;
using Multiplayer.Models;
using Multiplayer.Models.Movement;

namespace Chess.Movement
{
    [Serializable]
    public class MoveGroup
    {
        public MoveTypes Type;
        public Overtake Overtake;
        public bool CanJump;
        public bool Active = true;

        public MoveGroup(MoveTypes moveTypes, Overtake overtake, bool canJump)
        {
            Type = moveTypes;
            Overtake = overtake;
            CanJump = canJump;
        }
        
        public MoveGroup(MoveTypes moveTypes, Overtake overtake, bool canJump, bool active)
        {
            Type = moveTypes;
            Overtake = overtake;
            CanJump = canJump;
            Active = active;
        }

    }
}