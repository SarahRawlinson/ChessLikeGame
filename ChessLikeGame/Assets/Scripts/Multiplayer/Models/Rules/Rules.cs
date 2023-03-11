namespace Multiplayer.Models.Rules
{
    public class Rules
    {
        private  MultiGameStateData _gameStateData;
        public static int boardSize;
        
        public Rules(MultiGameStateData game)
        {
            _gameStateData = game;
            boardSize = game.GetGameBoardList().Count;
        }
        
        public bool ValidateMove(Move move)
        { return false;
        }

        public  bool CheckMoveViolations(Move move)
        {
            //Check Bounds
            if (move.StartPosition < 0 || move.StartPosition >= _gameStateData.GetGameBoardList().Count ||
                move.EndPosition < 0 || move.EndPosition > _gameStateData.GetGameBoardList().Count)
            { return false;
            }
            //Check if an emtpy square moves
            if (_gameStateData.GetGameBoardList()[move.StartPosition].pieceOnGrid.GetType() == ChessPieceTypes.NONE)
            { return false;
            }
            //Check if Same Team moves;
            if (_gameStateData.GetGameBoardList()[move.StartPosition].pieceOnGrid.Colour ==
                _gameStateData.GetGameBoardList()[move.EndPosition].pieceOnGrid.Colour)
            { return false;
            }

           
            
            return true;
        }
        
        
        
        
        public static int incrementX(int position, int delta){ return position + delta; }
        public static int decrementX(int position,int delta){ return position - delta; }
        public static int incrementY(int position,int delta){
            return position + (delta *8);
        }
        public static int decrementY(int position,int delta){
            return position - (delta *8);
        }
    }
}