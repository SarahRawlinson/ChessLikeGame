using System;
using System.Collections.Generic;
using Chess.Enums;
using Chess.Interface;
using Chess.Board;
using Chess.Control;
using Chess.Movement;
using UnityEngine;

namespace Chess.Pieces
{
    public class ChessPiece: MonoBehaviour, ICondition
    {
        [SerializeField] private GameObject pieceObject;
        internal List<MoveGroup> MovesGroupList = new List<MoveGroup>();
        internal List<Moves> MovesList = new List<Moves>();
        protected bool enPassant = false;
        public Vector2 pos;
        public Team team;
        internal BoardObject _board;
        internal string NameType = "Generic";
        internal string Key = "G";
        protected internal string EnPassantString = "";
        internal event Action<((int x, int y) from, (int x, int y) to)>  OnMove;
        private bool isActive = true;
        [SerializeField] internal int pieceValue;
        public List<ChessPiece> captured = new List<ChessPiece>();
        public Controller PieceController;
        internal (int x, int y) _startPosition;
        internal event Action OnTaken;
        public static event Action<ChessPiece, bool, Controller> OnSelectedChessPiece;
        public static event Action<ChessPiece> OnChosenChessPiece;
        public static event Action<GameObject, Team> TeamSwitch;
        public static event Action<ChessPiece, int> OnSwap;
        private bool SpecialMoveUsed = false;
        private bool _copy = false;
        [SerializeField] public MeshRenderer MeshRender;
        public bool HasMoved { get; set; }

        public void SetUpCopy(ChessPiece chessPiece)
        {
            transform.position = chessPiece.transform.position;
            team = chessPiece.team;
            pos = chessPiece.pos;
            captured = chessPiece.captured;
            PieceController = chessPiece.PieceController;
            _startPosition = chessPiece._startPosition;
            _board = chessPiece._board;
            _copy = true;
            TeamColour();
            SetPosition();
        }
        
        public virtual int ReturnNewPiece()
        {
            return 0;
        }
        
        internal void SwapPiece(ChessPiece oldPiece, int newPiece)
        {
            OnSwap?.Invoke(oldPiece, newPiece);
        }

        public void SwitchTeams()
        {
            PieceController.ChessPieceTaken(this);
            PieceController = PieceController.otherPlayer;
            team = PieceController._team;
            PieceController.PiecesCallToController(this);
            TeamColour();
        }

        public virtual void SpecialActions(int x, int y, Position positionFrom, Position positionTo)
        {
            
        }

        private void TeamColour()
        {
            TeamSwitch?.Invoke(pieceObject, team);
        }

        protected virtual void Awake()
        {
            HasMoved = false;
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
            DeactivateGameObject();
            OnTaken?.Invoke();
            OnTakenStatic?.Invoke();
        }

        public void DeactivateGameObject()
        {
            PieceController.ChessPieceTaken(this);
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
        }

        public void CapturePiece(ChessPiece piece)
        {
            piece.OnCaptured();
            captured.Add(piece);
            Debug.Log($"{team.ToString()} {this.NameType} takes {piece.team.ToString()} {piece.NameType}");
        }

        public virtual void SetPosition()
        {
            var posObj = GetPosition();
            _startPosition = posObj.GetPos();
            // posObj.SetPiece(this);
            _board.SetPosition(this, GetPositionXY());
        }

        public Position GetPosition()
        {
            return _board.Cubes[(int) pos.x][(int) pos.y];
        }

        public void Move((int x, int y) from, (int x, int y) to)
        {
            Position positionFrom = _board.GetPosition(from);
            Position positionTo = _board.GetPosition(from);
            HasMoved = true;
            OnMove?.Invoke((from, to));
            SpecialActions(to.x, to.y, positionFrom, positionTo);
        }

        public void Move(Position position)
        {
            HasMoved = true;
        }

        public void SetActive()
        {
            
        }
        
        
        public virtual void WorkOutMoves()
        {
            
        }

        public void OnMouseDown()
        {
            OnSelectedChessPiece?.Invoke(this, IsActive(), PieceController);
        }

        public List<Moves> GetPossibleMoves(Controller con)
        {
            List<Moves> possibleMoves = new List<Moves>();
            if (!isActive) return possibleMoves;
            MovesGroupList = new List<MoveGroup>();
            WorkOutMoves();
            foreach (MoveGroup m in MovesGroupList)
            {
                m.Active = true;
            }
            _board.ClearBoard();

            for (int index = 0; index < MovesList.Count; index++)
            {
                Moves move = MovesList[index];
                
                if (!MovesGroupList[move.GroupIndex].Active) continue;
                (int posX, int posY) = GetPos(move);
                if (move.IsCastle)
                {
                    // Debug.Log("castle move added");
                    move.MoveResultPos =  (posX, posY);
                    possibleMoves.Add(move);
                    _board.GetPosition((posX, posY)).Activate(this, con, move);
                    continue;
                }
                if (posX >= _board.Cubes.Count || posX < 0 || posY >= _board.Cubes[posX].Count || posY < 0)
                {
                    // Debug.Log($"Move Out Of Range {move.MoveType.ToString()} x={posX},y={posY}");
                    if (!(move.MoveType is MoveTypes.L))
                    {
                        MovesGroupList[move.GroupIndex].Active = false;
                    }

                    continue;
                }
                
                (int x, int y) posObj = (posX, posY);
                bool isTaken = _board.IsTaken(posObj);
                if (isTaken || _board.IsEnPassant($"{Position.Number2String(posX)}{posY-1}"))
                {
                    if (move.Overtake == Overtake.No)
                    {
                        MovesGroupList[move.GroupIndex].Active = false;
                        // Debug.Log($"Blocked Can't Overtake {move.MoveType.ToString()}");
                        continue;
                    }

                    if (_board.GetPosition(posObj).GetPiece().team == team)
                    {
                        if (!(move.MoveType is MoveTypes.L))
                        {
                            MovesGroupList[move.GroupIndex].Active = false;
                        }

                        // Debug.Log($"Blocked by Team {move.MoveType.ToString()}");
                        continue;
                    }

                    if (!(move.MoveType is MoveTypes.L))
                    {
                        MovesGroupList[move.GroupIndex].Active = false;
                    }
                }

                if (!isTaken && move.Overtake == Overtake.Yes) continue;
                // Debug.Log($"Path {move.MoveType.ToString()} ok");
                _board.GetPosition(posObj).Activate(this, con, move);
                if (isTaken)
                {
                    move.MoveValue = _board.GetPosition(posObj).GetPiece().pieceValue;
                }
                move.MoveResultPos =  posObj;
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
            // if (!isActive) Debug.Log($"{NameType} no longer active");
            // if (!PieceController.IsActive()) Debug.Log($"{team.ToString()} no longer active");
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
                    case MoveTypes.CastleKingSideBlack:
                        // Debug.Log("CastleKingSideBlack added");
                        movesList.Add(new Moves(3, 0, MovesGroupList[index], index, this, _board.GetPosition("H8").GetPiece()));
                        break;
                    case MoveTypes.CastleKingSideWhite:
                        // Debug.Log("CastleKingSideWhite added");
                        movesList.Add(new Moves(3, 0, MovesGroupList[index], index, this, _board.GetPosition("H1").GetPiece()));
                        break;
                    case MoveTypes.CastleQueenSideBlack:
                        // Debug.Log("CastleQueenSideBlack added");
                        movesList.Add(new Moves(-4, 0, MovesGroupList[index], index, this, _board.GetPosition("A8").GetPiece()));
                        break;
                    case MoveTypes.CastleQueenSideWhite:
                        // Debug.Log("CastleQueenSideWhite added");
                        movesList.Add(new Moves(-4, 0, MovesGroupList[index], index, this, _board.GetPosition("A1").GetPiece()));
                        break;
                }
            }

            return movesList;
        }

        public virtual bool GetConditionsMet(MoveTypes move, int step, Overtake overtake, bool jump)
        {
            Debug.Log($"Generic conditions met {NameType}");
            return true;
        }

        public (int x, int y) GetPositionXY()
        {
            return ((int)pos.x,(int) pos.y);
        }

        public static event Action OnTakenStatic;

        public virtual void WorkoutIfMoved()
        {
            Debug.Log($"Generic workout if moved {NameType}");
        }
    }
}