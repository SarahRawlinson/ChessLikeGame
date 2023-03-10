using System;
using Chess.Pieces;
using Multiplayer;
using Multiplayer.Models;

[Serializable]
public class ChessGrid
{
    public MultiPiece pieceOnGrid;
    private GridColor _gridColor;
    private int location;
    public static string[] columns = new[]
    {
        "A", "B", "C", "D", "E", "F", "G", "H"
    };
    
    // public ChessGrid(){}

    public ChessGrid(MultiPiece pieceOnGrid, int location)
    {
        this.pieceOnGrid = pieceOnGrid;
        _gridColor = CalculateGridColorFromLocation(location);
        this.location = location;
    }
    
    
    private GridColor CalculateGridColorFromLocation(int location) {

        int x, y;

        x = location % 8;
        y = location / 8;

        if ((x % 2) == (y % 2)) {
            return GridColor.BLACK;
        } else return GridColor.WHITE;
    }
    public static  (int x, int y) CalculateXYFromIndex(int index) {

        int x, y;

        x = index % 8;
        y = index / 8;

        return (x, y);
    }

    public static int CalculateIndexFromXY(int x, int y)
    {
        return x * y;
    }

    public string GetKey()
    {
        (int x, int y) = CalculateXYFromIndex(location);
        return $"{columns[x]}{(y+1).ToString()}";
    }
    
}


