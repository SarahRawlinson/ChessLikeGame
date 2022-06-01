using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chess.Enums;
using Chess.Interface;
using Chess.Board;
using Chess.Control;
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
        private bool isActive = true;
        [SerializeField] internal int pieceValue;
        public List<ChessPiece> captured = new List<ChessPiece>();
        public Controller PieceController;
        private Position _startPosition;
        internal event Action OnTaken;

        private void Awake()
        {
            _board = FindObjectOfType<BoardObject>();
            _board.OnBoardSetUp += SetPosition;
            FindObjectOfType<Director>().OnStart += GetController;
        }

        private void GetController()
        {
            foreach (Controller con in FindObjectsOfType<Controller>())
            {
                if (con.GetTeam() == team)
                {
                    PieceController = con;
                    con.PiecesCallToController(this);
                }
            }
        }

        private void OnCaptured()
        {
            isActive = false;
            if (TryGetComponent(out Renderer r))
            {
                r.enabled = false;
            }
            if (TryGetComponent(out Collider c))
            {
                c.enabled = false;
            }
            foreach (Transform obj in GetComponentsInChildren<Transform>())
            {
                if (obj.TryGetComponent(out Renderer or))
                {
                    or.enabled = false;
                }
                if (obj.TryGetComponent(out Collider oc))
                {
                    oc.enabled = false;
                }
            }
            OnTaken?.Invoke();
        }

        public void CapturePiece(ChessPiece piece)
        {
            piece.OnCaptured();
            captured.Add(piece);
            Debug.Log($"{team.ToString()} {this.NameType} takes {piece.team.ToString()} {piece.NameType}");
        }

        public void SetPosition()
        {
            var posObj = GetPosition();
            _startPosition = posObj;
            posObj.SetPiece(this);
        }

        public Position GetPosition()
        {
            Position posObj = _board._cubes[(int) pos.x][(int) pos.y];
            return posObj;
        }


        public void Move(Vector3 moveTransform, Vector2 nextPos)
        {
            GetPosition().MovePiece();
            OnMove?.Invoke();
            transform.position = new Vector3(moveTransform.x, transform.position.y, moveTransform.z);
            pos = nextPos;
            PieceController.MoveMade();
            _board.ClearBoard();
        }
        
        public IEnumerator AIMove(Moves moves)
        {
            yield return new WaitForSeconds(1);
            moves.MoveResultPos.OnAIMove(this);
        }

        public virtual void WorkOutMoves()
        {
            
        }

        public void OnMouseDown()
        {
            if (IsActive())
            {
                PieceController.LegalMovesList(GetPossibleMoves(PieceController));
            }
            else
            {
                Position position = GetPosition();
                if (position.IsActive()) position.MoveMade();
            }
            
        }

        public List<Moves> GetPossibleMoves(Controller con)
        {
            List<Moves> possibleMoves = new List<Moves>();
            if (!isActive) return possibleMoves;
            WorkOutMoves();
            foreach (MoveGroup m in MovesGroupList)
            {
                m.Active = true;
            }
            _board.ClearBoard();

            for (int index = 0; index < MovesList.Count; index++)
            {
                Moves move = MovesList[index];
                
                if (!MovesGroupList[move.groupIndex].Active) continue;
                (int posX, int posY) = GetPos(move);
                    
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
                if (posObj.IsTaken())
                {
                    move.PieceTaken = posObj.piece;
                    move.moveValue = posObj.piece.pieceValue;
                }
                move.MoveResultPos = posObj;
                possibleMoves.Add(move);
            }
            return possibleMoves;
        }

        (int x, int y) GetPos(Moves move)
        {
            int posX = (int) pos.x + move.Forward;
            int posY = (int) pos.y + move.Right;
            return (posX, posY);
        }

        
        public bool IsActive()
        {
            if (!isActive) Debug.Log($"{NameType} no longer active");
            if (!PieceController.IsActive()) Debug.Log($"{team.ToString()} no longer active");
            return isActive && PieceController.IsActive();
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
                        movesList.Add(new Moves(-step * offset, 0, MovesGroupList[index], index, this));
                        break;
                    case MoveTypes.Left:
                        movesList.Add(new Moves(step * offset, 0, MovesGroupList[index], index, this));
                        break;
                    case MoveTypes.Forward:
                        movesList.Add(new Moves(0, step * offset, MovesGroupList[index], index, this));
                        break;
                    case MoveTypes.Backward:
                        movesList.Add(new Moves(0, -step * offset, MovesGroupList[index], index, this));
                        break;
                    case MoveTypes.DiagonalDownLeft:
                        movesList.Add(new Moves(-step * offset, -step * offset, MovesGroupList[index], index, this));
                        break;
                    case MoveTypes.DiagonalDownRight:
                        movesList.Add(new Moves(step * offset, -step * offset, MovesGroupList[index], index, this));
                        break;
                    case MoveTypes.DiagonalUpLeft:
                        movesList.Add(new Moves(-step * offset, step * offset, MovesGroupList[index], index, this));
                        break;
                    case MoveTypes.DiagonalUpRight:
                        movesList.Add(new Moves(step * offset, step * offset, MovesGroupList[index], index, this));
                        break;
                    case MoveTypes.L:
                        movesList.Add(new Moves(1, 2, MovesGroupList[index], index, this));
                        movesList.Add(new Moves(-1, 2, MovesGroupList[index], index, this));
                        movesList.Add(new Moves(1, -2, MovesGroupList[index], index, this));
                        movesList.Add(new Moves(-1, -2, MovesGroupList[index], index, this));
                        movesList.Add(new Moves(2, 1, MovesGroupList[index], index, this));
                        movesList.Add(new Moves(-2, 1, MovesGroupList[index], index, this));
                        movesList.Add(new Moves(2, -1, MovesGroupList[index], index, this));
                        movesList.Add(new Moves(-2, -1, MovesGroupList[index], index, this));
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