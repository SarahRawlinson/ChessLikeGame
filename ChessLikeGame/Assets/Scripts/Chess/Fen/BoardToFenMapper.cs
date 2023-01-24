using System;
using System.Collections.Generic;
using System.Text;
using Chess.Board;
using Chess.Control;
using Chess.Enums;
using Chess.Pieces;
using Unity.VisualScripting;
using UnityEngine;

namespace Chess.Fen
{
    class BoardToFenMapper
    {
        private Dictionary<string, Position> _board = new Dictionary<string, Position>();
        private string _map = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        private List<string> _enPassantAllowed = new List<string>();

        public string GetMap()
        {
            return _map;
        }
        
        public BoardToFenMapper(List<Position> board, Controller nextPlayer, Director director)
        {
            StringBuilder mapStringBuilder = new StringBuilder();
            CreateBoardMap(board, ref mapStringBuilder);
            // Debug.Log(mapStringBuilder.ToString());
            mapStringBuilder.Append($" {nextPlayer.GetKey()} ");
            WorkOutCastle(ref mapStringBuilder);
            mapStringBuilder.Append(" ");
            WorkOutEnPassant(ref mapStringBuilder);
            mapStringBuilder.Append($" {director.HalfmoveClock} {director.FullMove}");
            _map = mapStringBuilder.ToString();
        }

        private void WorkOutEnPassant(ref StringBuilder mapStringBuilder)
        {
            _enPassantAllowed.Sort();
            bool enPassantFound = false;
            foreach (var pos in _enPassantAllowed)
            {
                mapStringBuilder.Append(pos);
                enPassantFound = true;
            }

            if (!enPassantFound)
            {
                mapStringBuilder.Append("-");
            }
        }

        private void WorkOutCastle(ref StringBuilder mapStringBuilder)
        {
            Position position;
            bool canMove = CheckCastle(ref mapStringBuilder, 1);
            if (CheckCastle(ref mapStringBuilder, 8)) canMove = true;
            if (!canMove) mapStringBuilder.Append("-");
        }

        private bool CheckCastle(ref StringBuilder mapStringBuilder, int row)
        {
            bool canCastle = false;
            Position position = _board[$"E{row}"];
            bool kingNoteMoved = false;
            if (position._isTaken)
            {
                ChessPiece piece = position.piece;
                if (piece.Key == "K" && !piece.HasMoved)
                {
                    kingNoteMoved = true;
                }
            }

            if (kingNoteMoved)
            {
                position = _board[$"H{row}"];
                if (position._isTaken)
                {
                    ChessPiece piece = position.piece;
                    if (piece.Key == "R" && !piece.HasMoved)
                    {
                        mapStringBuilder.Append(row == 1 ? "K": "k");
                        canCastle = true;
                    }
                }
                
                position = _board[$"A{row}"];
                if (position._isTaken)
                {
                    ChessPiece piece = position.piece;
                    if (piece.Key == "R" && !piece.HasMoved)
                    {
                        mapStringBuilder.Append(row == 1 ? "Q": "q");
                        canCastle = true;
                    }
                }
            }

            return canCastle;
        }

        private void CreateBoardMap(List<Position> board, ref StringBuilder mapStringBuilder)
        {
            foreach (Position position in board)
            {
                _board.Add(position.GetCoordinates(), position);
                // Debug.Log(position.GetCoordinates());
                if (position.IsEnPassant())
                {
                    var enPassantString = position.GetEnPassantString();
                    _enPassantAllowed.Add(enPassantString);
                }
            }

            BoardToFenMapper boardObj = this;
            int freeSpaces = 0;

            for (int i = 8; i > 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    string key = $"{Position.Number2String(j)}{i}";
                    if (!_board.ContainsKey(key))
                    {
                        throw new ArgumentException($"Board does not follow chess board rule no {Position.Number2String(i)}{j}",
                            nameof(boardObj));
                    }

                    Position pos = _board[key];
                    if (!pos._isTaken)
                    {
                        freeSpaces++;
                    }
                    else
                    {
                        ChessPiece piece = pos.piece;
                        if (freeSpaces > 0)
                        {
                            mapStringBuilder.Append(freeSpaces.ToString());
                            freeSpaces = 0;
                        }
                        if (piece.team == Team.Black)
                        {
                            mapStringBuilder.Append(piece.Key.ToLower());
                        }
                        else if (piece.team == Team.White)
                        {
                            mapStringBuilder.Append(piece.Key);
                        }
                        else
                        {
                            throw new ArgumentException($"Piece must have team x={i}, y={j} piece={piece.tag}",
                                nameof(boardObj));
                        }
                    }
                }

                if (freeSpaces > 0)
                {
                    mapStringBuilder.Append(freeSpaces.ToString());
                    freeSpaces = 0;
                }

                if (i > 1)
                {
                    mapStringBuilder.Append("/");
                }
            }
        }
    }
}