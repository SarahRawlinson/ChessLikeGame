using System;
using Chess.Pieces;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Chess.Board
{
    
    public class Position 
    {
        public ChessPiece piece = null;
        public Vector2 grid;
        private bool _isTaken = false;
        private ChessPiece isActiveForChessPiece;
        private bool isActive;
        private int GetX => (int)grid.x;
        private int GetY => (int)grid.y;
        private readonly PositionGameObject _positionObject;
        private readonly bool _hypothetical;

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
        
        public void SetPiece(ChessPiece obj)
        {
            // if (!obj.IsActive()) return;
            if (_isTaken)
            {
                obj.CapturePiece(piece);
            }
            _isTaken = true;
            piece = obj;
        }

        public void Activate(ChessPiece piece)
        {
            piece.OnMove += Deactivate;
            isActiveForChessPiece = piece;
            if (!_hypothetical) _positionObject._rend.enabled = true;
            if (!_hypothetical) _positionObject._collider.enabled = true;
            isActive = true;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void Deactivate()
        {
            if (isActive) isActiveForChessPiece.OnMove -= Deactivate;
            isActive = false;
            isActiveForChessPiece = null;
            if (!_hypothetical) _positionObject._rend.enabled = false;
            if (!_hypothetical) _positionObject._collider.enabled = false;
        }

        public void MovePiece()
        {
            piece = null;
            _isTaken = false;
        }

        public void MoveMade()
        {
            SetPiece(isActiveForChessPiece);
            if (!_hypothetical)
            {
                var pos = _positionObject.transform.position;
                isActiveForChessPiece.Move(new Vector3(pos.x, 0, pos.z), grid, this);
            }
            Deactivate();
        }

        public void OnAIMove(ChessPiece piece)
        {
            isActiveForChessPiece = piece;
            MoveMade();
        }

        public bool IsTaken()
        {
            return _isTaken;
        }

        public ChessPiece GetPiece()
        {
            return piece;
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
        
        private String Number2String(int number)

        {
            string[] columns = new[]
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                "V", "W", "X", "Y", "Z"
            };
            return columns[number];

        }
    }
}