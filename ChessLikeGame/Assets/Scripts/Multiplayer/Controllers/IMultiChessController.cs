using Chess.Board;

namespace Multiplayer.Controllers
{
    public interface IMultiChessController
    {
        public bool SetInformation(BoardStateData board);
        public bool SetActive(bool active);
        public (bool ready, bool errors) MoveChosen();
        public (int moveFromIndex, int moveToIndex) GetMove();
    }
}