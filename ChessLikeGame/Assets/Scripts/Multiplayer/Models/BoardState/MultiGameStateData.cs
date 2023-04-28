using System;
using System.Collections.Generic;
using System.Text;
using Multiplayer.Models.Movement;
using UnityEngine;

namespace Multiplayer.Models.BoardState
{
    [Serializable]
    public class MultiGameStateData
    {
        public  TeamColor ActivePlayer;
        public int EnPassantSquare; 
        public bool WhiteCanKingsideCastle;
        public bool WhiteCanQueensideCastle;
        public bool BlackCanKingsideCastle;
        public bool BlackCanQueensideCastle;
        public int HalfMoveCounter;
        public int FullMoveNumber;
        public List<MultiPiece> whitePiece = new List<MultiPiece>();
        public List<MultiPiece> blackPiece = new List<MultiPiece>();

    }
}
