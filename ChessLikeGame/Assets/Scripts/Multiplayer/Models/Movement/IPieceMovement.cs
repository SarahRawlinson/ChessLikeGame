using System.Collections.Generic;
using Multiplayer.Models.Movement;

namespace Multiplayer.Models.Rules
{
    public interface IPieceMovement
    {
        public List<MoveToValidate> possibleMoves();
        
    }
}