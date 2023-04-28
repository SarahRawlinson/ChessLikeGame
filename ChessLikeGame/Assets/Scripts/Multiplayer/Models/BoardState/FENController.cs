using System;
using System.Text;

namespace Multiplayer.Models.BoardState
{
    public class FENController
    {
        private MultiGameStateData currentDataSet;
        private ChessEngine engine;
        
        public FENController(MultiGameStateData metaData, ChessEngine enginePointer)
        {
            currentDataSet = metaData;
            engine = enginePointer;
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
                (int x, int y) = ChessGrid.CalculateXYFromIndex(counter);
                y = 7 - y;
                int index = ChessGrid.CalculateIndexFromXY(x, y);
                
                MultiPiece tmpPiece = new MultiPiece();
                ChessGrid chessGrid = engine.GetGameBoardList()[index];
                
                if (Char.IsLetter(character))
                {
                    WorkOutCharacter(tmpPiece, character);
                }
                else
                {
                    tmpPiece.SetKey("");
                }
                // Debug.Log($"Adding Piece to Board: {tmpPiece.Colour} {tmpPiece.GetPieceType()}  / Index: {index} / key: {chessGrid.GetKey()}, XY: ({x},{y})");
                
                chessGrid.SetPieceOnGrid(tmpPiece);

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

        private void WorkOutCharacter(MultiPiece tmpPiece, char character)
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
                currentDataSet.blackPiece.Add(tmpPiece);
            }
            else
            {
                tmpPiece.Colour = TeamColor.White;
                currentDataSet.whitePiece.Add(tmpPiece);
            }
        }

        private void LoadActivePlayerFromString(string fenString)
        {
            switch (fenString)
            {
                case "w":
                   currentDataSet. ActivePlayer = TeamColor.White;
                    break;
                case "b":
                    currentDataSet.ActivePlayer = TeamColor.Black;
                    break;
            }
        }
        
        private void LoadEnPassantSquare(string enPassantSquareString)
        {
            if (enPassantSquareString == "-")
            {
                currentDataSet. EnPassantSquare = -1;
            }
            else if (!String.IsNullOrEmpty(enPassantSquareString))
            {
               currentDataSet. EnPassantSquare = ChessGrid.GetIndexFromKey(enPassantSquareString);
            }
        }
        
        private void LoadCastlingFromString(string castlingAvailabilityString)
        {
           currentDataSet. BlackCanKingsideCastle = false;
           currentDataSet.BlackCanQueensideCastle = false;
           currentDataSet.WhiteCanKingsideCastle = false;
           currentDataSet.WhiteCanQueensideCastle = false;

            if (castlingAvailabilityString != "-")
            {
                if (castlingAvailabilityString.Contains("K"))
                {
                    currentDataSet. WhiteCanKingsideCastle = true;
                }

                if (castlingAvailabilityString.Contains("k"))
                {
                    currentDataSet. BlackCanKingsideCastle = true;
                }

                if (castlingAvailabilityString.Contains("Q"))
                {
                    currentDataSet.WhiteCanQueensideCastle = true;
                }

                if (castlingAvailabilityString.Contains("q"))
                {
                    currentDataSet.BlackCanQueensideCastle = true;
                }
            }
        }
        
        private void LoadHalfMoveCounterFromString(string halfmoveClockString)
        {
            currentDataSet.HalfMoveCounter = int.Parse(halfmoveClockString);
        }
        
        private void LoadFullmoveNumberFromString (string fullmoveNumberString)
        {
            currentDataSet. FullMoveNumber = int.Parse(fullmoveNumberString);
        }
        
        #endregion

        #region GenerateFen

        private string FenBuilder()
        {
            return $"{GetBoardString()} {GetActivePlayerString()} {GetCastleString()} {GetEnPassentString()} {currentDataSet.HalfMoveCounter.ToString()} {currentDataSet.FullMoveNumber.ToString()}";
        }

        private string GetEnPassentString()
        {
            if (currentDataSet.EnPassantSquare < 0)
            {
                return "-";
            }
            return engine.GetGameBoardList()[currentDataSet.EnPassantSquare].GetKey();
        }

        private string GetActivePlayerString()
        {
            return  currentDataSet.ActivePlayer == TeamColor.Black ? "b" : "w";
        }

        private string GetCastleString()
        {
            string K = currentDataSet.WhiteCanKingsideCastle ? "K" : "";
            string Q = currentDataSet.WhiteCanQueensideCastle ? "Q" : "";
            string k = currentDataSet.BlackCanKingsideCastle ? "k" : "";
            string q = currentDataSet.BlackCanQueensideCastle ? "q" : "";
            string castle = $"{K}{Q}{k}{q}";
            return castle;
        }

        public MultiPiece GetPieceFromPositionOnBoard(int index)
        {
            return engine.GetGameBoardList()[index].PieceOnGrid;
        }

        private string GetBoardString()
        {
            StringBuilder mapStringBuilder = new StringBuilder();
            int freeSpaces = 0;

            for (int i =engine.GetGameBoardList().Count -1; i >= 0; i--)
            {
                ChessGrid pos = engine.GetGameBoardList()[i];
                MultiPiece piece = pos.PieceOnGrid;
                if (piece.GetPieceType() == ChessPieceTypes.NONE)
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