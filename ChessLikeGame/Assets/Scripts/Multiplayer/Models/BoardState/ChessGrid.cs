using System;

namespace Multiplayer.Models.BoardState
{
    
[Serializable]
public class ChessGrid
{
    private MultiPiece pieceOnGrid;
    private GridColor _gridColor;
    private int location;
    static readonly string[] Columns = new[]{"A", "B", "C", "D", "E", "F", "G", "H"};

    public MultiPiece PieceOnGrid => pieceOnGrid;
    public GridColor GridColor => _gridColor;
    public int Location => location;


    public ChessGrid(MultiPiece pieceOnGrid, int location)
    {
        this.pieceOnGrid = pieceOnGrid;
        _gridColor = CalculateGridColorFromLocation(location);
        this.location = location;
    }

    public static  GridColor CalculateGridColorFromLocation(int location) 
    {
        int x, y;
        x = location % 8;
        y = location / 8;
        if ((x % 2) == (y % 2)) {
            return GridColor.BLACK;
        } else return GridColor.WHITE;
    }
    
    public static  (int x, int y) CalculateXYFromIndex(int index) 
    {
        int x, y;
        x = index % 8;
        y = index / 8;
        return (x, y);
    }

    public static int CalculateIndexFromXY(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
        {
            return -1;
        }
        return (y * 8) + x ;
    }

    public string GetKey()
    {
        return GetKeyFromIndex(location);
    }

    public static int GetIndexFromKey(string key)
    {
        (int x, int y) = CalculateXYFromKey(key);
        return CalculateIndexFromXY(x, y);
    }
    
    public static  (int x, int y) CalculateXYFromKey(string key) {

        var x = Array.IndexOf(Columns, key[0].ToString());
        var y_str = key[1].ToString();
        var y = (Int32.Parse(y_str)-1);
        return (x, y);
    }
    
    public static string GetKeyFromIndex(int location)
    {
        (int x, int y) = CalculateXYFromIndex(location);
        return $"{Columns[x]}{(y+1).ToString()}";
    }

    public void SetPieceOnGrid(MultiPiece tmpPiece)
    {
        pieceOnGrid = tmpPiece;
    }
}



}