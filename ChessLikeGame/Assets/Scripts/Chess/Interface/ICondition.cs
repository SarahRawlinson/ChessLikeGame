using Chess.Enums;
using Multiplayer.Models;
using Multiplayer.Models.Movement;

namespace Chess.Interface
{
    public interface ICondition
    {
        bool GetConditionsMet(MoveTypes move, int step, Overtake overtake, bool jump);
    }
}