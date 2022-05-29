using System;
using Chess.Pieces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Chess.Board
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Collider))]
    public class Position : MonoBehaviour
    {
        public ChessPiece piece = null;
        public Vector2 grid;
        private MeshRenderer _rend;
        private Collider _collider;
        private bool _isTaken = false;
        private ChessPiece isActiveForChessPiece;
        private bool isActive;

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
            _rend.enabled = true;
            _collider.enabled = true;
            isActive = true;
        }

        public void Deactivate()
        {
            if (isActive) isActiveForChessPiece.OnMove -= Deactivate;
            isActive = false;
            isActiveForChessPiece = null;
            _rend.enabled = false;
            _collider.enabled = false;
        }

        public void MovePiece()
        {
            piece = null;
            _isTaken = false;
        }

        private void OnMouseDown()
        {
            MoveMade();
        }

        private void MoveMade()
        {
            SetPiece(isActiveForChessPiece);
            var pos = transform.position;
            isActiveForChessPiece.Move(new Vector3(pos.x, 0, pos.z), grid);
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

        private void Awake()
        {
            _rend = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
        }
    }
}