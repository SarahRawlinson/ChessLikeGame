using System;
using Chess.Pieces;
using Multiplayer;

[Serializable]
public class ChessGrid
{
    private Piece pieceOnGrid;
    private GridColor _gridColor;

    
    private GridColor CalculateGridColorFromLocation(int location) {

        int x, y;

        x = location % 8;
        y = location / 8;

        if ((x % 2) == (y % 2)) {
            return GridColor.BLACK;
        } else return GridColor.WHITE;
    }
}


