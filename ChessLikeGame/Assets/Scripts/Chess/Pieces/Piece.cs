using System;
using System.Collections.Generic;
using Chess.Control;
using Chess.Movement;

namespace Chess.Pieces
{
    public class Piece
    {
        public static readonly List<Move> Moves = new List<Move>();
        public Piece(int x, int y, double value, char label, Boolean team, char colour, Boolean hasMoved = false)
        {
            X = x;
            Y = y;
            Value = value;
            Label = label;
            IsPlayer1 = team;
            _hasMoved = hasMoved;
            Colour = colour;
        }


        public int X { get; set; }

        public int Y { get; set; }

        public double Value { get; set; }

        public char Label { get; set; }

        public char Colour { get; set; }

        public Boolean IsPlayer1 { get; set; }

        private bool _hasMoved;
        public Boolean HasMoved => HasMovedPlace();

        public bool HasMovedPlace()
        {
            foreach (var pMove in Moves)
            {
                if (pMove.Piece.Label == Label && pMove.DestX == X && pMove.DestY == Y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsPromotion(Move moveToMake)
        {
            if (Label.ToString().ToLower() == "p")
            {
                if (moveToMake.DestY == 7 && Colour == 'w')
                {
                    return true;
                }
                if (moveToMake.DestY == 0 && Colour == 'b')
                {
                    return true;
                }
            }
            return false;
        }
    }
}