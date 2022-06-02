using UnityEditorInternal;
using UnityEngine;

namespace Chess.Board
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Collider))]
    public class PositionGameObject : MonoBehaviour
    {
        public MeshRenderer _rend;
        public Collider _collider;
        private Position _position;

        public PositionGameObject()
        {
            _position = new Position(this);
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
        private void OnMouseDown()
        {
            _position.MoveMade();
        }
    }
}