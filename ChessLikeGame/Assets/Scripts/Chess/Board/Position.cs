﻿using System;
using System.Linq;
using System.Text;
using Chess.Control;
using Chess.Enums;
using Chess.Fen;
using Chess.Movement;
using Chess.Pieces;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Chess.Board
{
    [Serializable]
    public class Position 
    {
        public ChessPiece piece = null;
        public Vector2 grid;
        public bool _isTaken = false;
        public Moves PossibleMove = null;
        public readonly PositionGameObject _positionObject;
        private ChessPiece isActiveForChessPiece;
        private bool isActive;
        public bool enPassant = false;
        private int GetX => (int)grid.x;
        private int GetY => (int)grid.y;
        private readonly bool _hypothetical;
        public static string[] columns = new[]
        {
            "A", "B", "C", "D", "E", "F", "G", "H"
        };

        public ChessPiece GetPiece()
        {
            return piece;
        }

        public Position(PositionGameObject positionGameObject)
        {
            _hypothetical = false;
            _positionObject = positionGameObject;
        }

        public Position(Position position)
        {
            _hypothetical = true;
            piece = position.piece;
            grid = position.grid;
            _isTaken = position._isTaken;
            isActiveForChessPiece = position.isActiveForChessPiece;
            isActive = position.isActive;
            _positionObject = position._positionObject;
        }

        public GameObject SpawnPiece(GameObject obj)
        {
            return _positionObject.SpawnGameObject(obj);
        }
        
        // public void SetPiece(ChessPiece obj)
        // {
        //     // if (!obj.IsActive()) return;
        //     if (_isTaken)
        //     {
        //         // obj.CapturePiece(piece);
        //     }
        //     _isTaken = true;
        //     piece = obj;
        // }

        public void Activate(ChessPiece piece, Controller con, Moves possibleMove)
        {
            PossibleMove = possibleMove;
            // Debug.Log($"move set {GetCoordinates()} {piece.NameType}");
            bool aiCon = con.TryGetComponent(out AI ai);
            // piece.OnMove += Deactivate;
            isActiveForChessPiece = piece;
            if (!_hypothetical && !aiCon)
            {
                _positionObject._rend.enabled = true;
                _positionObject._collider.enabled = true;
                // Debug.Log($"{GetCoordinates()} Activated by {piece.team.ToString()} {piece.NameType}");
            }
            isActive = true;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void Deactivate()
        {
            // Debug.Log($"move removed {GetCoordinates()}");
            PossibleMove = null;
            // if (isActive) isActiveForChessPiece.OnMove -= Deactivate;
            isActive = false;
            isActiveForChessPiece = null;
            _positionObject._rend.enabled = false;
            _positionObject._collider.enabled = false;
        }

        public bool IsTaken()
        {
            return _isTaken;
        }

        public string GetCoordinates()
        {
            return $"{GetAlphabets((int) grid.x)}{(int)(grid.y + 1)}";
        }

        public override bool Equals(object other)
        {
            //Check for null and compare run-time types.
            if ((other == null) || this.GetType() != other.GetType())
            {
                return false;
            }
            else {
                Position p = (Position) other;
                return (GetX == p.GetX) && (GetY == p.GetY);
            }
        }

        public string GetAlphabets(int num)
        { 
            string strAlpha = "";
            int alp = 26;
            int letters = (num / alp);
            int remainder = (num % alp);
            strAlpha = Number2String(remainder);

            while (letters / (alp) > 0)
            {
                letters = (letters / (alp));
                int l = (letters % (alp));
                strAlpha = $"{Number2String(l)}{strAlpha}";
            }
            return strAlpha;
        }
        
        public static String Number2String(int number)
        {
            return columns[number];
        }
        
        public static int String2Number(string number)
        {
            return Array.IndexOf(columns,number);
        }

        public (int x, int y) GetPos()
        {
            return (GetX, GetY);
        }

        public Team Team()
        {
            if (!_isTaken) return Enums.Team.Null;
            return piece.team;
        }

        public float PieceValue()
        {
            if (!_isTaken) return 0;
            return piece.pieceValue;
        }

        public void RemovePiece()
        {
            piece = null;
            _isTaken = false;
        }

        public void SetEnPassant(bool on)
        {
            enPassant = on;
        }

        public bool IsEnPassant()
        {
            return enPassant;
        }

        public string GetEnPassantString()
        {
            if (_isTaken)
            {
                return piece.EnPassantString;
            }
            return "";
        }

        public void PlacePiece(ChessPiece newPositionPiece)
        {
            Debug.Log("place piece");
            piece = newPositionPiece;
            _positionObject.SetPiece(newPositionPiece);
        }

        public void SetPiece(ChessPiece chessPiece)
        {
            Debug.Log("set piece");
            piece = chessPiece;
            _isTaken = true;
        }

    }
}