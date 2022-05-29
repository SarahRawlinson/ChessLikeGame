using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Enums;
using Chess.Interface;
using Chess.Board;
using Chess.Movement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Chess.Pieces
{
    public class ChessPiece: MonoBehaviour, ICondition
    {
        internal List<MoveGroup> MovesGroupList = new List<MoveGroup>();
        internal List<Moves> MovesList = new List<Moves>();
        public Vector2 pos;
        public Team team;
        internal BoardObject _board;
        internal string NameType = "Generic";
        internal event Action OnMove;
        public List<ChessPiece> captured = new List<ChessPiece>();

        private void Awake()
        {
            _board = FindObjectOfType<BoardObject>();
            _board.OnBoardSetUp += SetPosition;
        }

        public void CapturePiece(ChessPiece piece)
        {
            captured.Add(piece);
        }

        public void SetPosition()
        {
            Position posObj = _board._cubes[(int) pos.x ][(int) pos.y ];
            posObj.SetPiece(this);
        }
        

        public void Move(Transform moveTransform, Vector2 nextPos)
        {
            _board._cubes[(int) pos.x ][(int) pos.y ].MovePiece();
            OnMove?.Invoke();
            transform.position = moveTransform.position;
            pos = nextPos;
        }

        public virtual void WorkOutMoves()
        {
            
        }

        public void OnMouseDown()
        {
            WorkOutMoves();
            // Debug.Log(NameType);
            for (var index = 0; index < MovesGroupList.Count; index++)
            {
                MovesGroupList[index].Active = true;
            }

            foreach (List<Position> posList in _board._cubes)
            {
                foreach (Position pos in posList)
                {
                    pos.Deactivate();
                }
            }

            for (int index = 0; index < MovesList.Count; index++)
            {
                Moves move = MovesList[index];
                int posX = (int) pos.x + move.Forward;
                int posY = (int) pos.y + move.Right;
                try
                {
                    if (!MovesGroupList[move.groupIndex].Active) continue;
                    if (posX >= _board._cubes.Count || posX < 0 || posY >= _board._cubes[posX].Count || posY < 0)
                    {
                        // Debug.Log($"Move Out Of Range {move.MoveType.ToString()} x={posX},y={posY}");
                        if (!(move.MoveType is MoveTypes.L))
                        {
                            MovesGroupList[move.groupIndex].Active = false;
                        }
                        continue;
                    }
                    Position posObj = _board._cubes[posX][posY];
                    if (posObj.IsTaken())
                    {
                        if (move.Overtake == Overtake.No)
                        {
                            MovesGroupList[move.groupIndex].Active = false;
                            // Debug.Log($"Blocked Can't Overtake {move.MoveType.ToString()}");
                            continue;
                        }

                        if (posObj.piece.team == team)
                        {
                            if (!(move.MoveType is MoveTypes.L))
                            {
                                MovesGroupList[move.groupIndex].Active = false;
                            }
                            // Debug.Log($"Blocked by Team {move.MoveType.ToString()}");
                            continue;
                        }
                        if (!(move.MoveType is MoveTypes.L))
                        {
                            MovesGroupList[move.groupIndex].Active = false;
                        }
                    }

                    if (!posObj.IsTaken() && move.Overtake == Overtake.Yes) continue;
                    // Debug.Log($"Path {move.MoveType.ToString()} ok");
                    posObj.Activate(this);
                }
                catch (Exception e)
                {
                    Debug.Log($"Error x={posX},y={posY}:{e}");
                    continue;
                }
            }
        }
        
        internal void WorkOutMovesUnlimitedSteps()
        {
            MovesList.Clear();
            int numberOfMoves;
            if (_board.columns > _board.rows)
            {
                numberOfMoves = _board.columns;
            }
            else
            {
                numberOfMoves = _board.rows;
            }

            for (int i = 0; i < numberOfMoves; i++)
            {
                MovesList.AddRange(GetMoves(i + 1));
            }
        }

        public List<Moves> GetMoves(int step)
        {
            List<Moves> movesList = new List<Moves>();
            for (var index = 0; index < MovesGroupList.Count; index++)
            {
                if (!GetConditionsMet(MovesGroupList[index].Type, step, MovesGroupList[index].Overtake,
                    MovesGroupList[index].CanJump)) continue;
                int offset = 0;
                if (team == Team.Black) offset = -1;
                else offset = 1;
                switch (MovesGroupList[index].Type)
                {
                    case MoveTypes.Right:
                        movesList.Add(new Moves(-step * offset, 0, MovesGroupList[index], index));
                        break;
                    case MoveTypes.Left:
                        movesList.Add(new Moves(step * offset, 0, MovesGroupList[index], index));
                        break;
                    case MoveTypes.Forward:
                        movesList.Add(new Moves(0, step * offset, MovesGroupList[index], index));
                        break;
                    case MoveTypes.Backward:
                        movesList.Add(new Moves(0, -step * offset, MovesGroupList[index], index));
                        break;
                    case MoveTypes.DiagonalDownLeft:
                        movesList.Add(new Moves(-step * offset, -step * offset, MovesGroupList[index], index));
                        break;
                    case MoveTypes.DiagonalDownRight:
                        movesList.Add(new Moves(step * offset, -step * offset, MovesGroupList[index], index));
                        break;
                    case MoveTypes.DiagonalUpLeft:
                        movesList.Add(new Moves(-step * offset, step * offset, MovesGroupList[index], index));
                        break;
                    case MoveTypes.DiagonalUpRight:
                        movesList.Add(new Moves(step * offset, step * offset, MovesGroupList[index], index));
                        break;
                    case MoveTypes.L:
                        movesList.Add(new Moves(1, 2, MovesGroupList[index], index));
                        movesList.Add(new Moves(-1, 2, MovesGroupList[index], index));
                        movesList.Add(new Moves(1, -2, MovesGroupList[index], index));
                        movesList.Add(new Moves(-1, -2, MovesGroupList[index], index));
                        movesList.Add(new Moves(2, 1, MovesGroupList[index], index));
                        movesList.Add(new Moves(-2, 1, MovesGroupList[index], index));
                        movesList.Add(new Moves(2, -1, MovesGroupList[index], index));
                        movesList.Add(new Moves(-2, -1, MovesGroupList[index], index));
                        break;
                }
            }

            return movesList;
        }

        public virtual bool GetConditionsMet(MoveTypes move, int step, Overtake overtake, bool jump)
        {
            Debug.Log("Generic");
            return true;
        }
        
    }
}