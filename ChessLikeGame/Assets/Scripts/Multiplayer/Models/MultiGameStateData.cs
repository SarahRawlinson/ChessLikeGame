using System;
using System.Collections.Generic;
using System.Text;
using Multiplayer.Models.Movement;
using UnityEngine;

namespace Multiplayer.Models
{
    [Serializable]
    public class MultiGameStateData
    {
        private TeamColor ActivePlayer;
        private int EnPassantSquare;
        private List<ChessGrid> GameBoard = new List<ChessGrid>();
        private bool WhiteCanKingsideCastle;
        private bool WhiteCanQueensideCastle;
        private bool BlackCanKingsideCastle;
        private bool BlackCanQueensideCastle;
        private int HalfMoveCounter;
        private int FullMoveNumber;
        private List<MultiPiece> whitePiece = new List<MultiPiece>();
        private List<MultiPiece> blackPiece = new List<MultiPiece>();

        private Rules.Rules rules;
        
        public MultiGameStateData(string fen)
        {
            CreateBoard();
            SetUpBoardFromFen(fen);
            Debug.Log(FenBuilder());
        }

        public List<ChessGrid> GetGameBoardList()
        {
            return GameBoard;
        }

        private void CreateBoard()
        {
            int counter = 0;
            GameBoard.Clear();
            for (; counter < 64; counter++)
            {
                ChessGrid chessGrid = new ChessGrid(new MultiPiece(), counter);
                Debug.Log($"creating board position, index:{GameBoard.Count.ToString()}, position: {chessGrid.GetKey()}");
                GameBoard.Add(chessGrid);
            }
        }

        public bool MakeMoveOnBoard(Move move)
        {
            rules = new Rules.Rules(this);

            if (rules.isMoveValid(move))
            {
                //todo: move
                ChessGrid start = GameBoard[move.StartPosition];
                ChessGrid end = GameBoard[move.EndPosition];
                end.pieceOnGrid = start.pieceOnGrid;
                start.pieceOnGrid = new MultiPiece();
                return true;
            }
            return false;
        }


        #region LoadFromFen
        
        public void SetUpBoardFromFen(string fen)
        {
            var processed = fen.Split(" ");
            LoadPositionsFromString(processed[0]);
            LoadActivePlayerFromString(processed[1]);
            LoadCastlingFromString(processed[2]);
            LoadEnPassantSquare(processed[3]);
            LoadHalfMoveCounterFromString(processed[4]);
            LoadFullmoveNumberFromString(processed[5]);
        }
        public void LoadPositionsFromString(string inputFEN)
        {
            int counter = 0;
            foreach (var character in inputFEN)
            {
                MultiPiece tmpPiece = new MultiPiece();
                ChessGrid chessGrid = GameBoard[63-counter];
                (int x, int y) calculateXYFromIndex = ChessGrid.CalculateXYFromIndex(counter);
                tmpPiece.SetXY(calculateXYFromIndex.x, calculateXYFromIndex.y);
                if (Char.IsLetter(character))
                {
                    tmpPiece.SetKey(character.ToString());
                    switch (character.ToString().ToUpper())
                    {
                        case "P":
                            tmpPiece.SetType(ChessPieceTypes.PAWN);
                            break;
                        case "R":
                            tmpPiece.SetType(ChessPieceTypes.ROOK);
                            break;
                        case "N":
                            tmpPiece.SetType(ChessPieceTypes.KNIGHT);
                            break;
                        case "B":
                            tmpPiece.SetType(ChessPieceTypes.BISHOP);
                            break;
                        case "Q":
                            tmpPiece.SetType(ChessPieceTypes.QUEEN);
                            break;
                        case "K":
                            tmpPiece.SetType(ChessPieceTypes.KING);
                            break;
                    }
                    if (Char.IsLower(character))
                    {
                        tmpPiece.Colour = TeamColor.Black;
                        blackPiece.Add(tmpPiece);
                    }
                    else
                    {
                        tmpPiece.Colour = TeamColor.White;
                        whitePiece.Add(tmpPiece);
                    }
                    
                }
                else
                {
                    tmpPiece.SetKey("");
                }

                
                Debug.Log($"Adding Piece to Board: {tmpPiece.Colour} {tmpPiece.GetType()}  / Index: {63-counter} / key: {chessGrid.GetKey()}");
                
                chessGrid.pieceOnGrid = tmpPiece;

                if (Char.IsDigit(character))
                {
                    counter += (int) Char.GetNumericValue(character) - 1;
                }

                if (character != '/')
                {
                    counter++;
                }
            }
        }
        
        private void LoadActivePlayerFromString(string fenString)
        {
            switch (fenString)
            {
                case "w":
                    ActivePlayer = TeamColor.White;
                    break;
                case "b":
                    ActivePlayer = TeamColor.Black;
                    break;
            }
        }
        
        private void LoadEnPassantSquare(string enPassantSquareString)
        {
            if (enPassantSquareString == "-")
            {
                EnPassantSquare = -1;
            }
            else if (!String.IsNullOrEmpty(enPassantSquareString))
            {
                EnPassantSquare = ChessGrid.GetIndexFromKey(enPassantSquareString);
            }
        }
        
        private void LoadCastlingFromString(string castlingAvailabilityString)
        {
            BlackCanKingsideCastle = false;
            BlackCanQueensideCastle = false;
            WhiteCanKingsideCastle = false;
            WhiteCanQueensideCastle = false;

            if (castlingAvailabilityString != "-")
            {
                if (castlingAvailabilityString.Contains("K"))
                {
                    WhiteCanKingsideCastle = true;
                }

                if (castlingAvailabilityString.Contains("k"))
                {
                    BlackCanKingsideCastle = true;
                }

                if (castlingAvailabilityString.Contains("Q"))
                {
                    WhiteCanQueensideCastle = true;
                }

                if (castlingAvailabilityString.Contains("q"))
                {
                    BlackCanQueensideCastle = true;
                }
            }
        }
        
        private void LoadHalfMoveCounterFromString(string halfmoveClockString)
        {
            HalfMoveCounter = int.Parse(halfmoveClockString);
        }
        
        private void LoadFullmoveNumberFromString (string fullmoveNumberString)
        {
            FullMoveNumber = int.Parse(fullmoveNumberString);
        }
        
        #endregion

        #region GenerateFen

        private string FenBuilder()
        {
            return $"{GetBoardString()} {GetActivePlayerString()} {GetCastleString()} {GetEnPassentString()} {HalfMoveCounter.ToString()} {FullMoveNumber.ToString()}";
        }

        private string GetEnPassentString()
        {
            if (EnPassantSquare < 0)
            {
                return "-";
            }
            return GameBoard[EnPassantSquare].GetKey();
        }

        private string GetActivePlayerString()
        {
            return ActivePlayer == TeamColor.Black ? "b" : "w";
        }

        private string GetCastleString()
        {
            string K = WhiteCanKingsideCastle ? "K" : "";
            string Q = WhiteCanQueensideCastle ? "Q" : "";
            string k = BlackCanKingsideCastle ? "k" : "";
            string q = BlackCanQueensideCastle ? "q" : "";
            string castle = $"{K}{Q}{k}{q}";
            return castle;
        }

        public MultiPiece GetPieceFromPositionOnBoard(int index)
        {
            return GameBoard[index].pieceOnGrid;
        }

        private string GetBoardString()
        {
            StringBuilder mapStringBuilder = new StringBuilder();

            int freeSpaces = 0;

            for (int i = GameBoard.Count -1; i >= 0; i--)
            {
                ChessGrid pos = GameBoard[i];
                MultiPiece piece = pos.pieceOnGrid;
                if (piece.GetType() == ChessPieceTypes.NONE)
                {
                    freeSpaces++;
                }
                else
                {
                    if (piece.Colour == TeamColor.Black)
                    {
                        mapStringBuilder.Append(piece.GetKey().ToLower());
                    }
                    else if (piece.Colour == TeamColor.White)
                    {
                        mapStringBuilder.Append(piece.GetKey().ToUpper());
                    }
                    else
                    {
                        throw new ArgumentException($"Piece must have team index={i} piece={piece.GetKey()}");
                    }
                }

                (int x, int y) calculateXYFromIndex = ChessGrid.CalculateXYFromIndex(i);
                if (calculateXYFromIndex.x == 0)
                {
                    if (freeSpaces > 0)
                    {
                        mapStringBuilder.Append(freeSpaces.ToString());
                        freeSpaces = 0;
                    }
                    if (i > 0)
                    {
                        mapStringBuilder.Append("/");
                    }
                }
                
            }

            return mapStringBuilder.ToString();
        }
        
        #endregion
        
    }

    
    
}
