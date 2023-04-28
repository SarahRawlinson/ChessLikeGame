using System.Collections.Generic;
using Chess.Pieces;
using Multiplayer.Models.Movement;
using UnityEngine;

namespace Multiplayer.Models.BoardState
{
    public class ChessEngine
    {
        private List<ChessGrid> _gameBoard = new List<ChessGrid>();
        private Rules.Rules _rules;
        private MultiGameStateData _gameStateData = new MultiGameStateData();
        private FENController _fenController;
        public ChessEngine()
        {
            _fenController = new FENController(_gameStateData, this);
            _rules = new Rules.Rules(this);
        }

        public Rules.Rules GetRules()
        {
            return _rules;
        }
        
        
        public List<ChessGrid> GetGameBoardList()
        {
            return _gameBoard;
        }

        public FENController GetFenController()
        {
            return _fenController;
        }
        
       public void CreateBoard()
        {
            
            ChessGrid.SetBoardWidth(8);
            
            int counter = 0;
            _gameBoard.Clear();
            for (; counter < 64; counter++)
            {
                // (int x, int y) = ChessGrid.CalculateXYFromIndex(counter);
                ChessGrid chessGrid = new ChessGrid(new MultiPiece(), counter);
                // (int bx, int by) = ChessGrid.CalculateXYFromKey(chessGrid.GetKey());
                // int checkIndex = ChessGrid.CalculateIndexFromXY(x, y);
                // Debug.Log($"creating board position, index:{GameBoard.Count.ToString()}/{checkIndex}, position: {chessGrid.GetKey()}, aXY: ({x},{y}), bXY: ({bx},{by})");
                _gameBoard.Add(chessGrid);
            }
        }

       public bool MakeMoveOnBoard(Move move)
       {
           if (_rules.isMoveValid(move))
           {
                //Clear out the enemy piece
               if (_gameBoard[move.EndPosition].PieceOnGrid.GetPieceType() != ChessPieceTypes.NONE)
               {
                   _gameBoard[move.EndPosition].SetPieceOnGrid(null); //Todo: Do something with the dead piece in future
                   _gameBoard[move.EndPosition].SetPieceOnGrid( _gameBoard[move.StartPosition].PieceOnGrid);
                   _gameBoard[move.StartPosition].SetPieceOnGrid(new MultiPiece()) ;
                   
               }
               return true;    
           }
           return false;
       }

        
        
    }
}