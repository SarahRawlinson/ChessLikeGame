using System;
using System.Collections.Generic;
using Chess.Control;
using Chess.Enums;
using Chess.Interface;
using Chess.Movement;
using Chess.Pieces;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Chess.Board
{
    public class BoardObject : MonoBehaviour
    {
        public readonly List<List<Position>> Cubes = new List<List<Position>>();
        public int rows = 10;
        public int columns = 10;
        [SerializeField] private PositionGameObject cube;
        private Vector3 _cubeSize;
        public event Action OnBoardSetUp;
        private King[] _kings;
        private Controller[] _controllers;
        private List<(ChessPiece piece, int xNewPosIndex, int yNewPosIndex, int xOldPosIndex, int yOldPosIndex)> moves;

        private void Awake()
        {
            _kings = FindObjectsOfType<King>();
            _controllers = FindObjectsOfType<Controller>();
        }

        public List<List<Position>> GetHypotheticalBoard()
        {
            List<List<Position>> posList = new List<List<Position>>();
            foreach (List<Position> pList in Cubes)
            {
                List<Position> newList = new List<Position>();
                foreach (Position p in pList)
                {
                    newList.Add(new Position(p, this));
                }
                posList.Add(newList);
            }
            return posList;
        }
        
        public void Move(Vector3 moveTransform, Vector2 nextPos, (int x, int y) position, ChessPiece piece)
        {
            (int x, int y)  positionFrom = piece.GetPosition();
            RemovePiece(positionFrom);
            piece.Move();
            piece.transform.position = new Vector3(moveTransform.x, transform.position.y, moveTransform.z);
            piece.pos = nextPos;
            piece.PieceController.MoveMade();
            ClearBoard();
            Debug.Log($"{piece.team.ToString()} {piece.NameType} moves from {GetCoordinates(positionFrom)} to {GetCoordinates(position)}");
        }

        public void Move(Moves move)
        {
            Move(GetPositionVector(move.MoveResultPos), new Vector2(move.MoveResultPos.x, move.MoveResultPos.y),
                move.MoveResultPos, move.Piece);
        }

        Vector3 GetPositionVector((int x, int y) pos)
        {
            return Cubes[pos.x][pos.y]._positionObject.transform.position;
        }

        public void ClearBoard()
        {
            foreach (List<Position> posList in Cubes)
            {
                foreach (Position pos in posList)
                {
                    pos.Deactivate();
                }
            }
        }
        private void Start()
        {
            GameObject cubeObj = cube.gameObject;
            _cubeSize = cubeObj.transform.localScale;
            for (int c = 0; c < columns; c++)
            {
                List<Position> objects = new List<Position>();
                for (int r = 0; r < rows; r++)
                {
                    GameObject obj = Instantiate(cubeObj, transform);
                    obj.transform.position = (transform.position - new Vector3(0,(_cubeSize.y / 2),0)) -
                        new Vector3((_cubeSize.x * c), 0, (_cubeSize.z * r));
                    PositionGameObject posObj = obj.GetComponent<PositionGameObject>();
                    objects.Add(posObj.GetPosition());
                    objects[r].grid = new Vector2(c, r);
                    posObj.SetText();
                }
                Cubes.Add(objects);
            }

            OnBoardSetUp?.Invoke();
        }

        private void StoreAction((int x, int y) pos, (int x, int y) oldPos)
        {
            moves.Add((Cubes[pos.x][pos.y].piece, pos.x, pos.y,oldPos.x, oldPos.y ));
        }

        public void SetPosition(ChessPiece chessPiece, (int x, int y) getPosition)
        {
            // Debug.Log("SetPosition");
            Cubes[getPosition.x][getPosition.y].piece = chessPiece;
        }

        public void RemovePiece((int x, int y) move)
        {
            Debug.Log("RemovePiece");
            Cubes[move.x][move.y].RemovePiece();
        }

        public string GetCoordinates((int x, int y) positionFrom)
        {
            return Cubes[positionFrom.x][positionFrom.y].GetCoordinates();
        }

        public bool IsActive((int x, int y) position)
        {
            Debug.Log("IsActive");
            return Cubes[position.x][position.y].IsActive();
        }

        public void MoveSelected((int x, int y) position)
        {
            Debug.Log("MoveSelected");
            Cubes[position.x][position.y].MoveSelected();
            // throw new NotImplementedException();
        }

        public bool IsTaken((int x, int y) posObj)
        {
            return Cubes[posObj.x][posObj.y].IsTaken();
        }

        public Team Team((int x, int y) posObj)
        {
            return Cubes[posObj.x][posObj.y].Team();
        }

        public void Activate(ChessPiece chessPiece, Controller con, (int x, int y) posObj)
        {
            // Debug.Log("Activate");
            Cubes[posObj.x][posObj.y].Activate(chessPiece, con);
        }

        public float PieceValue((int x, int y) posObj)
        {
            return Cubes[posObj.x][posObj.y].PieceValue();
        }
        
        public List<Moves> LegalMovesList(List<Moves> movesList)
        {
            if (IsCheck())
            {
                MoveStopsCheck(ref movesList);
            }
            return movesList;
        }
        
        //todo work this out
        private void MoveStopsCheck(ref List<Moves> movesList)
        {
            
        }
        bool IsCheck()
        {
            foreach (Controller con in _controllers)
            {
                if (con.IsCheck()) return true;
            }
            return false;
        }
    }
}