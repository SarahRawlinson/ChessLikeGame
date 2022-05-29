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

        public void SetPiece(ChessPiece obj)
        {
            if (_isTaken)
            {
                obj.CapturePiece(piece);
                foreach (MeshRenderer renderer in piece.gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.enabled = false;
                }
            }
            _isTaken = true;
            piece = obj;
        }

        public void Activate(ChessPiece piece)
        {
            isActiveForChessPiece = piece;
            _rend.enabled = true;
            _collider.enabled = true;
        }

        public void Deactivate()
        {
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
            SetPiece(isActiveForChessPiece);
            isActiveForChessPiece.Move(transform, grid);
            Deactivate();
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