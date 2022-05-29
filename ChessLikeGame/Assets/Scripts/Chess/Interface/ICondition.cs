using Chess.Enums;

namespace Chess.Interface
{
    public interface ICondition
    {
        bool GetConditionsMet(MoveTypes move, int step, Overtake overtake, bool jump);
    }
}