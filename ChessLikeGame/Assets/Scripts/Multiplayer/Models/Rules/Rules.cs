using System;
using System.Collections.Generic;
using Multiplayer.Models.Movement;

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
        
        public List<Move> ValidateMove(MoveToValidate toValidate, TeamColor color, int startPos)
        {
            int forward = color == TeamColor.White ? 1 : -1;
            (int x, int y) basePos = ChessGrid.CalculateXYFromIndex(startPos);
            List<Move> moves = new List<Move>();
            //Todo: fill moves list, this is just the template
            int steps = toValidate.NumberOfSteps;
            int endPos;
            switch (toValidate.MoveType)
            {
                case MoveTypes.Forward:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + (forward * steps), basePos.y);
                    moves.Add(new Move(startPos, endPos, color));
                    break;
                case MoveTypes.Backward:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - (forward * steps), basePos.y);
                    moves.Add(new Move(startPos, endPos, color));
                    break;
                case MoveTypes.Left:
                    break;
                case MoveTypes.Right:
                    break;
                case MoveTypes.DiagonalUpRight:
                    break;
                case MoveTypes.DiagonalUpLeft:
                    break;
                case MoveTypes.DiagonalDownRight:
                    break;
                case MoveTypes.DiagonalDownLeft:
                    break;
                case MoveTypes.L:
                    break;
                case MoveTypes.CastleKingSide:
                    break;
                case MoveTypes.CastleQueenSide:
                    break;
                case MoveTypes.CastleQueenSideWhite:
                case MoveTypes.CastleQueenSideBlack:
                case MoveTypes.CastleKingSideWhite:
                case MoveTypes.CastleKingSideBlack:
                    throw new ArgumentOutOfRangeException(
                        nameof(toValidate), 
                        "specified castle depreciated use MoveTypes.CastleKingSide or MoveTypes.CastleQueenSide instead"
                    );
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (Move move in moves)
            {
                foreach (var validate in toValidate.Validators)
                {
                    switch (validate)
                    {
                        case MoveValidationTypes.CheckForClearPath:
                            break;
                        case MoveValidationTypes.CheckEmpty:
                            break;
                        case MoveValidationTypes.CheckOccupied:
                            break;
                        case MoveValidationTypes.CheckKingCantBeTaken:
                            break;
                        case MoveValidationTypes.CheckHasNotMoved:
                            break;
                        case MoveValidationTypes.CheckKingSideRookHasNotMoved:
                            break;
                        case MoveValidationTypes.CheckQueenSideRookHasNotMoved:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            return moves;
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