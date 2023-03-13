using System;
using System.Collections.Generic;
using Multiplayer.Models.Movement;
using UnityEngine;

namespace Multiplayer.Models.Rules
{
    public class Rules
    {
        private  MultiGameStateData _gameStateData;
        public static int boardSize;

        public static int BoardSize
        {
            get => boardSize;
            set => boardSize = value;
        }

        public Rules(MultiGameStateData game)
        {
            SetGameStateData(game);
        }

        public bool isMoveValid(Move moveToTest)
        {
            MultiPiece piece =
                _gameStateData.GetGameBoardList()[moveToTest.StartPosition].PieceOnGrid;
            foreach (var move in GetMovesByPiece(piece))
            {
                if (Move.CheckEqual(move, moveToTest))
                {
                    return true;
                }
            }
            return false;
        }

        private void SetGameStateData(MultiGameStateData game)
        {
            _gameStateData = game;
            boardSize = game.GetGameBoardList().Count;
        }

        public List<Move> GetMovesByPiece(MultiPiece piece)
        {
            List<Move> moves = new List<Move>();
            IPieceMovement movement;
            switch (piece.GetPieceType())
            {
                case ChessPieceTypes.NONE:
                    return moves;
                case ChessPieceTypes.PAWN:
                    movement = new PawnMoves();
                    break;
                case ChessPieceTypes.BISHOP:
                    movement = new BishopMoves();
                    break;
                case ChessPieceTypes.KNIGHT:
                    movement = new KnightMoves();
                    break;
                case ChessPieceTypes.ROOK:
                    movement = new RookMoves();
                    break;
                case ChessPieceTypes.QUEEN:
                    movement = new QueenMoves();
                    break;
                case ChessPieceTypes.KING:
                    movement = new KingMoves();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            List<MoveToValidate> movesToValidate = movement.possibleMoves();
            foreach (var moveToValidate in movesToValidate)
            {
                moves.AddRange(GenerateValidatedMoves(moveToValidate, piece.Colour, ChessGrid.CalculateIndexFromXY(piece.X, piece.Y)));
            }
            return moves;
        }
        
        public List<Move> GenerateValidatedMoves(MoveToValidate toValidate, TeamColor color, int startPos)
        {
            (int x, int y) basePos = ChessGrid.CalculateXYFromIndex(startPos);
            List<Move> moves = new List<Move>();
            int steps = toValidate.NumberOfSteps;
            GetMovesByType(toValidate.MoveType, color, startPos, basePos, steps, moves);
            List<Move> validMoves = new List<Move>();
            foreach (Move move in moves)
            {
                bool canAdd = ValidateMove(toValidate, move);
                if (canAdd) validMoves.Add(move);
            }
            
            return validMoves;
        }

        private bool ValidateMove(MoveToValidate toValidate, Move move)
        {
            bool canAdd = true;
            if (move.HasSecondMove)
            {
                if (!ValidateMove(toValidate, move.SecondMove))
                {
                    return false;
                }
            }
            
            if (!CheckMoveViolations(move))
            {
                return false;
            }

            ChessGrid startPos = _gameStateData.GetGameBoardList()[move.StartPosition];
            ChessGrid endPos = _gameStateData.GetGameBoardList()[move.EndPosition];
            MultiPiece piece = startPos.PieceOnGrid;
            foreach (var validate in toValidate.Validators)
            {
                switch (validate)
                {
                    case MoveValidationTypes.CheckForClearPath:
                        //todo: need to work this out
                        break;
                    case MoveValidationTypes.CheckEmpty:
                        if (endPos.PieceOnGrid.GetPieceType() != ChessPieceTypes.NONE)
                        {
                            Debug.Log($"{move.MoveType}: {piece.GetPieceType()} {startPos.GetKey()} cant move to {endPos.GetKey()} square not empty");
                            return false;
                        }
                        break;
                    case MoveValidationTypes.CheckOccupied:
                        if (endPos.PieceOnGrid.GetPieceType() == ChessPieceTypes.NONE)
                        {
                            Debug.Log($"{move.MoveType}: {piece.GetPieceType()} {startPos.GetKey()} cant move to {endPos.GetKey()} square not occupied");
                            return false;
                        }
                        break;
                    case MoveValidationTypes.CheckKingCantBeTaken:
                        //todo: need to work this out
                        break;
                    case MoveValidationTypes.CheckHasNotMoved:
                        if (piece.HasMoved())
                        {
                            Debug.Log($"{move.MoveType}: {piece.GetPieceType()} {startPos.GetKey()} cant move to {endPos.GetKey()} has already moved");
                            return false;
                        }
                        break;
                    case MoveValidationTypes.CheckIsKingOrRook:
                        if (piece.GetPieceType() != ChessPieceTypes.KING && piece.GetPieceType() != ChessPieceTypes.ROOK)
                        {
                            Debug.Log($"{move.MoveType}: {piece.GetPieceType()} {startPos.GetKey()} cant move to {endPos.GetKey()} square does not contain King or Castle");
                            return false;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return canAdd;
        }

        private static void GetMovesByType(MoveTypes moveType, TeamColor color, int startPos, (int x, int y) basePos, 
            int steps, List<Move> moves)
        {
            int forwardDirection = color == TeamColor.White ? 1 : -1;
            int endPos;
            Move firstMove;
            switch (moveType)
            {
                case MoveTypes.Forward:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x , basePos.y + (forwardDirection * steps));
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    break;
                case MoveTypes.Backward:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x , basePos.y - (forwardDirection * steps));
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    break;
                case MoveTypes.Left:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - (forwardDirection * steps), basePos.y);
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    break;
                case MoveTypes.Right:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + (forwardDirection * steps), basePos.y);
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    break;
                case MoveTypes.DiagonalUpRight:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - (forwardDirection * steps), basePos.y + (forwardDirection * steps));
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    break;
                case MoveTypes.DiagonalUpLeft:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + (forwardDirection * steps), basePos.y + (forwardDirection * steps));
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    break;
                case MoveTypes.DiagonalDownRight:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - (forwardDirection * steps), basePos.y - (forwardDirection * steps));
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    break;
                case MoveTypes.DiagonalDownLeft:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + (forwardDirection * steps), basePos.y - (forwardDirection * steps));
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    break;
                case MoveTypes.L:
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + 2, basePos.y + 1);
                    PrintLMove(startPos, basePos, endPos);
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + 2, basePos.y - 1);
                    PrintLMove(startPos, basePos, endPos);
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + 1, basePos.y - 2);
                    PrintLMove(startPos, basePos, endPos);
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - 1, basePos.y - 2);
                    PrintLMove(startPos, basePos, endPos);
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - 2, basePos.y + 1);
                    PrintLMove(startPos, basePos, endPos);
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - 2, basePos.y - 1);
                    PrintLMove(startPos, basePos, endPos);
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x + 1, basePos.y + 2);
                    PrintLMove(startPos, basePos, endPos);
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    
                    endPos = ChessGrid.CalculateIndexFromXY(basePos.x - 1, basePos.y + 2);
                    PrintLMove(startPos, basePos, endPos);
                    moves.Add(new Move(moveType, startPos, endPos, color));
                    
                    break;
                case MoveTypes.CastleKingSide:
                    firstMove = new Move(moveType, 
                        ChessGrid.CalculateIndexFromXY(7, basePos.y),
                        ChessGrid.CalculateIndexFromXY(5, basePos.y),
                        color);
                    moves.Add(
                        new Move(moveType, 
                            ChessGrid.CalculateIndexFromXY(4, basePos.y),
                            ChessGrid.CalculateIndexFromXY(6, basePos.y),
                            color,
                            firstMove));
                    break;
                case MoveTypes.CastleQueenSide:
                    firstMove = new Move(moveType, 
                        ChessGrid.CalculateIndexFromXY(0, basePos.y),
                        ChessGrid.CalculateIndexFromXY(3, basePos.y),
                        color);
                    moves.Add(
                        new Move(moveType, 
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

        private static void PrintLMove(int startPos, (int x, int y) basePos, int endPos)
        {
            (int x, int y) tuple;
            tuple = ChessGrid.CalculateXYFromIndex(endPos);
            Debug.Log($"({basePos.x}, {basePos.y}), ({tuple.x},{tuple.y})");
            if (startPos < 0 || startPos > 63 || endPos < 0 || endPos > 63)
            {
                return;
            }
            Debug.Log($"({ChessGrid.GetKeyFromIndex(startPos)}), ({ChessGrid.GetKeyFromIndex(endPos)})");
        }

        public  bool CheckMoveViolations(Move move)
        {
            //Check Bounds
            if (move.StartPosition < 0 || move.StartPosition >= _gameStateData.GetGameBoardList().Count ||
                move.EndPosition < 0 || move.EndPosition > _gameStateData.GetGameBoardList().Count)
            { 
                Debug.Log($"{move.MoveType}: {move.StartPosition} cant move to {move.EndPosition} is out of bounds");
                return false;
            }
            ChessGrid startPos = _gameStateData.GetGameBoardList()[move.StartPosition];
            ChessGrid endPos = _gameStateData.GetGameBoardList()[move.EndPosition];
            //Check if an emtpy square moves
            if (_gameStateData.GetGameBoardList()[move.StartPosition].PieceOnGrid.GetPieceType() == ChessPieceTypes.NONE)
            { 
                Debug.Log($"{move.MoveType}: {startPos.GetKey()} cant move to {endPos.GetKey()} nothing to move");
                return false;
            }
            //Check if Same Team moves;
            if (_gameStateData.GetGameBoardList()[move.StartPosition].PieceOnGrid.Colour ==
                _gameStateData.GetGameBoardList()[move.EndPosition].PieceOnGrid.Colour)
            { 
                Debug.Log($"{move.MoveType}: {startPos.GetKey()} cant move to {endPos.GetKey()} square contains team piece");
                return false;
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