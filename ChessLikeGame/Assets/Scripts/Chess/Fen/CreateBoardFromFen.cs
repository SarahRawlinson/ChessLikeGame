using System.Collections.Generic;
using Chess.Board;
using Chess.Control;
using Chess.Enums;
using Chess.Pieces;
using Unity.VisualScripting;
using UnityEngine;

namespace Chess.Fen
{
    public class CreateBoardFromFen
    {
        private Director _director;
        public FenParser fenParser;

        public CreateBoardFromFen(Director director)
        {
            _director = director;
        }

        public void SetUpBoardFromFen(string fen)
        {
            fenParser = new FenParser(fen);
            var pieces = FenToListMapper.GetPieces(fenParser.BoardStateData);
            var WhitePieces = pieces.Item1;
            var BlackPieces = pieces.Item2;
            Dictionary<string, ChessPiece> piecesDic = new Dictionary<string, ChessPiece>();


            foreach (var piece in WhitePieces)
            {
                Team team = Team.White;
                var pieceString = CreatePieceString(piece, team);
                piecesDic.Add(pieceString.Key, pieceString.Value);
            }
            
            foreach (var piece in BlackPieces)
            {
                Team team = Team.Black;
                var pieceString = CreatePieceString(piece, team);
                piecesDic.Add(pieceString.Key, pieceString.Value);
            }
            
            
            KingHasMoved(
                piecesDic,
                true, 
                fenParser.BoardStateData.BlackCanKingsideCastle, 
                "h8", 
                fenParser.BoardStateData.BlackCanQueensideCastle, 
                "a8", 
                "e8");
            KingHasMoved(
                piecesDic,
                true, 
                fenParser.BoardStateData.WhiteCanKingsideCastle, 
                "h1", 
                fenParser.BoardStateData.WhiteCanQueensideCastle, 
                "a1", 
                "e1");
        }

        private static void KingHasMoved(Dictionary<string, ChessPiece> piecesDic, bool kingHasMoved, bool CanKingsideCastle, string rookKing,
            bool CanQueensideCastle, string rookQueen, string KingString)
        {
            if (!piecesDic.TryGetValue(KingString, out ChessPiece king))
            {
                return;
            }
            if (piecesDic.TryGetValue(rookKing, out ChessPiece rookK))
            {
                kingHasMoved = KingHasMoved(CanKingsideCastle, rookK, kingHasMoved);
            }
            if (piecesDic.TryGetValue(rookQueen, out ChessPiece rookQ))
            {
                kingHasMoved = KingHasMoved(CanQueensideCastle, rookQ, kingHasMoved);
            }
            king.HasMoved = kingHasMoved;
            if (kingHasMoved)
            {
                Debug.Log($"{king.NameType} has Moved");
            }
            else
            {
                King kingComponent = king.GetComponent<King>();
                kingComponent.CanCastleKing = CanKingsideCastle;
                kingComponent.CanCastleQueen = CanQueensideCastle;
            }
        }

        private static bool KingHasMoved(bool CanKingCastle, ChessPiece chessPiece, bool kingHasMoved)
        {
            if (CanKingCastle)
            {
                chessPiece.HasMoved = false;
                kingHasMoved = false;
            }
            else
            {
                chessPiece.HasMoved = true;
                Debug.Log($"{chessPiece.NameType} has Moved");
            }

            return kingHasMoved;
        }

        private KeyValuePair<string, ChessPiece> CreatePieceString(Piece piece, Team team)
        { 
            char lable = piece.Label;
            string name = "";
            switch (lable)
            {
                case 'K':
                    name = "king";
                    break;
                case 'P':
                    name = "pawn";
                    break;
                case 'R':
                    name = "rook";
                    break;
                case 'Q':
                    name = "queen";
                    break;
                case 'N':
                    name = "knight";
                    break;
                case 'B':
                    name = "bishop";
                    break;
            }

            string pos = $"{Position.Number2String(piece.X)}{piece.Y + 1}".ToLower();
            KeyValuePair<string, string> pieceString = new KeyValuePair<string, string>(pos, name);
            GameObject gameObject = _director.CreatePiece(pieceString, team);
            ChessPiece chessPiece = gameObject.GetComponent<ChessPiece>();
            chessPiece.WorkoutIfMoved();
            return new KeyValuePair<string, ChessPiece>(pos, chessPiece);
        }
    }
}