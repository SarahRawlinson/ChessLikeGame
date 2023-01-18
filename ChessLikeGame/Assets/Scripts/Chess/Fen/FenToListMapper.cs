using System;
using System.Collections.Generic;
using Chess.Board;
using Chess.Pieces;

namespace Chess.Fen
{
    public static class FenToListMapper
    {
        public static Tuple<List<Piece>, List<Piece>> GetPieces(BoardStateData board)
        {
            List<Piece> whitePieces = new List<Piece>();
            List<Piece> blackPieces = new List<Piece>();
            var pvm = new PieceValueMapper();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    char piece = board.Ranks[i][j][0];
                    if (piece == ' ')
                        continue;
                    if (piece < 97)
                        whitePieces.Add(new Piece(j, 7-i, pvm.GetValueForPiece(piece), piece, true, 'w'));
                    else
                        blackPieces.Add(new Piece(j, 7-i, pvm.GetValueForPiece(piece), piece.ToString().ToUpper()[0], false, 'b'));
                }

            }
            return new Tuple<List<Piece>, List<Piece>>(whitePieces, blackPieces);
        }
    }


}