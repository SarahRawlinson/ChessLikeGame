﻿using System;
using Chess.Pieces;
using TMPro;
using UnityEngine;

namespace Chess.Board
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Collider))]
    public class PositionGameObject : MonoBehaviour
    {
        public MeshRenderer _rend;
        public Collider _collider;
        [SerializeField] private Position _position;
        [SerializeField] public TMP_Text _text;
        [SerializeField] GameObject piecePosition;

        public static event Action<(int x, int y), PositionGameObject> OnSelected;

        public PositionGameObject()
        {
            _position = new Position(this);
            
        }
        public void MoveSelected()
        {
            OnSelected?.Invoke(_position.GetPos(), this);
        }
        public (int x, int y) GetPos()
        {
            return (_position.GetPos());
        }

        public Position GetPosition()
        {
            return _position;
        }

        private void Awake()
        {
            _rend = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
        }

        public void SetText()
        {
            _text.text = _position.GetCoordinates();
            gameObject.name = _position.GetCoordinates();
        }

        public GameObject SpawnGameObject(GameObject obj)
        {
            Vector3 pos = new Vector3(piecePosition.transform.position.x, piecePosition.transform.position.y, piecePosition.transform.position.z);
            return Instantiate(obj, pos, Quaternion.identity);
        }

        private void OnMouseDown()
        {
            MoveSelected();
        }

        public void SetPiece(ChessPiece newPositionPiece)
        {
            newPositionPiece.transform.position = new Vector3(transform.position.x, newPositionPiece.transform.position.y, transform.position.z);
            newPositionPiece.HasMoved = true;
            newPositionPiece.SetPosition();
        }
    }
}