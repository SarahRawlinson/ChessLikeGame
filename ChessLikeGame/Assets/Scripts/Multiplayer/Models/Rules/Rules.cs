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
            
            GetMovesByType(toValidate.MoveType, color, startPos, basePos, forward, steps, moves);
            List<Move> validMoves = new List<Move>();
            foreach (Move move in moves)
            {
                if (!CheckMoveViolations(move))
                {
                    continue;
                }
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
                validMoves.Add(move);
            }
            
            return validMoves;
        }

        private static void GetMovesByType(MoveTypes moveType, TeamColor color, int startPos, (int x, int y) basePos,
            int forward, int steps, List<Move> moves)
        {
            int endPos;
            Move firstMove;
            switch (moveType)
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
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x, basePos.y - (forward * steps));
                    moves.Add(new Move(startPos, endPos, color));
                    break;
                case MoveTypes.Right:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x, basePos.y + (forward * steps));
                    moves.Add(new Move(startPos, endPos, color));
                    break;
                case MoveTypes.DiagonalUpRight:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + (forward * steps), basePos.y + (forward * steps));
                    moves.Add(new Move(startPos, endPos, color));
                    break;
                case MoveTypes.DiagonalUpLeft:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - (forward * steps), basePos.y + (forward * steps));
                    moves.Add(new Move(startPos, endPos, color));
                    break;
                case MoveTypes.DiagonalDownRight:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + (forward * steps), basePos.y - (forward * steps));
                    moves.Add(new Move(startPos, endPos, color));
                    break;
                case MoveTypes.DiagonalDownLeft:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - (forward * steps), basePos.y - (forward * steps));
                    moves.Add(new Move(startPos, endPos, color));
                    break;
                case MoveTypes.L:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + 2, basePos.y + 1);
                    moves.Add(new Move(startPos, endPos, color));
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + 2, basePos.y - 1);
                    moves.Add(new Move(startPos, endPos, color));
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + 1, basePos.y - 2);
                    moves.Add(new Move(startPos, endPos, color));
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - 1, basePos.y - 2);
                    moves.Add(new Move(startPos, endPos, color));
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - 2, basePos.y + 1);
                    moves.Add(new Move(startPos, endPos, color));
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - 2, basePos.y - 1);
                    moves.Add(new Move(startPos, endPos, color));
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + 1, basePos.y + 2);
                    moves.Add(new Move(startPos, endPos, color));
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - 1, basePos.y + 2);
                    moves.Add(new Move(startPos, endPos, color));
                    break;
                case MoveTypes.CastleKingSide:
                    firstMove = new Move(
                        ChessGrid.CalculateIndexFromXY(basePos.x, 7),
                        ChessGrid.CalculateIndexFromXY(basePos.x, 5),
                        color);
                    moves.Add(
                        new Move(
                            ChessGrid.CalculateIndexFromXY(basePos.x, 4),
                            ChessGrid.CalculateIndexFromXY(basePos.x, 6),
                            color,
                            firstMove));
                    break;
                case MoveTypes.CastleQueenSide:
                    firstMove = new Move(
                        ChessGrid.CalculateIndexFromXY(basePos.x, 0),
                        ChessGrid.CalculateIndexFromXY(basePos.x, 3),
                        color);
                    moves.Add(
                        new Move(
                            ChessGrid.CalculateIndexFromXY(basePos.x, 4),
                            ChessGrid.CalculateIndexFromXY(basePos.x, 2),
                            color,
                            firstMove));
                    break;
                case MoveTypes.CastleQueenSideWhite:
                case MoveTypes.CastleQueenSideBlack:
                case MoveTypes.CastleKingSideWhite:
                case MoveTypes.CastleKingSideBlack:
                    throw new ArgumentOutOfRangeException(
                        nameof(moveType),
                        "specified castle depreciated use MoveTypes.CastleKingSide or MoveTypes.CastleQueenSide instead"
                    );
                default:
                    throw new ArgumentOutOfRangeException();
            }
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