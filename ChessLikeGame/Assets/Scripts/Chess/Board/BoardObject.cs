using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chess.Control;
using Chess.Enums;
using Chess.Fen;
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
        public class Actions
        {
            public ChessPiece piece;
            public int xNewPosIndex;
            public int yNewPosIndex;
            public int xOldPosIndex;
            public int yOldPosIndex;

            public Actions(ChessPiece p, int nx, int ny, int ox, int oy)
            {
                piece = p;
                xNewPosIndex = nx;
                yNewPosIndex = ny;
                xOldPosIndex = ox;
                yOldPosIndex = oy;
            }
        }

        private BoardStateData CurrentBoardStateData;
        public readonly List<List<Position>> Cubes = new List<List<Position>>();
        public int rows = 10;
        public int columns = 10;
        [SerializeField] private PositionGameObject cube;
        private Vector3 _cubeSize;
        public event Action OnBoardSetUp;
        private King[] _kings;
        private Controller[] _controllers;
        private List<Actions> moves = new List<Actions>();
        private ChessPiece activeChessPiece;
        private FenParser fenParser;

        private void Awake()
        {
            _kings = FindObjectsOfType<King>();
            _controllers = FindObjectsOfType<Controller>();
            ChessPiece.OnChosenChessPiece += OnChosenMove;
            ChessPiece.OnSelectedChessPiece += OnSelectedPiece;
            PositionGameObject.OnSelected += OnGridSelect;
            
        }

        

        private void OnGridSelect((int x, int y) obj, PositionGameObject posObj)
        {
            (int x, int y) p = posObj.GetPos();
            if (GetPosition(obj).IsActive()) Move(posObj.transform.position, new Vector2(p.x, p.y),p,activeChessPiece, false, GetPosition(p).PossibleMove.IsCastle);
        }

        private void OnSelectedPiece(ChessPiece obj, bool on, Controller con)
        {
            if (on) ActivatePositions(obj, con);
            else
            {
                (int x, int y) pos = obj.GetPositionXY();
                Position p = GetPosition(pos);
                OnGridSelect(pos, p._positionObject);
            };
        }

        private void ActivatePositions(ChessPiece obj, Controller con)
        {
            activeChessPiece = obj;
            obj.GetPossibleMoves(con);
        }

        private void OnChosenMove(ChessPiece obj)
        {
            Position p = GetPosition(obj.GetPositionXY());
            OnGridSelect(p.GetPos(),p._positionObject);
        }

        // public List<List<Position>> GetHypotheticalBoard()
        // {
        //     List<List<Position>> posList = new List<List<Position>>();
        //     foreach (List<Position> pList in Cubes)
        //     {
        //         List<Position> newList = new List<Position>();
        //         foreach (Position p in pList)
        //         {
        //             newList.Add(new Position(p));
        //         }
        //         posList.Add(newList);
        //     }
        //     return posList;
        // }
        
        public void Move(Vector3 moveTransform, Vector2 nextPos, (int x, int y) position, ChessPiece piece, bool callTaken, bool isCastle)
        {
            //TODO look at castle move here instead?
            (int x, int y) lastPos = piece.GetPositionXY();
            Position lastPosition = GetPosition(lastPos);
            Position newPosition = GetPosition(position);
            if(isCastle)
            {
                lastPosition.PlacePiece(newPosition.GetPiece());
                var king = piece.GetComponent<King>();
                king.CanCastleKing = false;
                king.CanCastleQueen = false;
            }
            else
            {
                if (Cubes[position.x][position.y].IsTaken())
                {
                    ChessPiece takenPiece = null;
                    bool taken = false;
                    taken = true;
                    takenPiece = Cubes[position.x][position.y].GetPiece();
                    piece.CapturePiece(takenPiece);
                }
                if (Cubes[position.x][position.y].IsEnPassant())
                {
                    ChessPiece takenPiece = null;
                    //3w 6b
                    Position position2 = Cubes[position.x][position.y==2?3:5];
                    Debug.Log(position2.GetCoordinates());
                    takenPiece = position2.GetPiece();
                    piece.CapturePiece(takenPiece);
                    RemovePiece(position2.GetPos());
                }
                if (!callTaken) RemovePiece(lastPos);
            }
            piece.transform.position = new Vector3(moveTransform.x, transform.position.y, moveTransform.z);
            piece.pos = nextPos;
            
            ClearBoard();
            Debug.Log($"{piece.team.ToString()} {piece.NameType} moves from {GetCoordinates(lastPos)} to {GetCoordinates(position)}");
            SetPosition(piece,position);
            StoreAction(piece,position.x, position.y, lastPos.x, lastPos.y);
            // if (taken && takenPiece != null)
            // {
            //     Debug.Log($"{takenPiece.NameType} taken moving to {GetCoordinates((lastPos.x,lastPos.y))}");
            //     Move(Cubes[lastPos.x][lastPos.y]._positionObject.transform.position,new Vector2(lastPos.x, lastPos.y) ,Cubes[lastPos.x][lastPos.y].GetPos(), takenPiece, true);
            // }
            piece.Move(lastPos, ((int)nextPos.x, (int)nextPos.y));
            piece.PieceController.MoveMade();
        }

        public void Move(Moves move)
        {
            Move(GetPositionVector(move.MoveResultPos), new Vector2(move.MoveResultPos.x, move.MoveResultPos.y),
                move.MoveResultPos, move.Piece, false, move.IsCastle);
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

        public void SetReady()
        {
            OnBoardSetUp?.Invoke();
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

            // OnBoardSetUp?.Invoke();
        }

        private void StoreAction(ChessPiece piece, int nx, int ny, int ox, int oy)
        {
            moves.Add(new Actions(piece, nx, ny, ox, oy ));
        }

        public void SetPosition(ChessPiece chessPiece, (int x, int y) getPosition)
        {
            Cubes[getPosition.x][getPosition.y].SetPiece(chessPiece);
        }

        public void RemovePiece((int x, int y) move)
        {
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

        public bool IsTaken((int x, int y) posObj)
        {
            return Cubes[posObj.x][posObj.y].IsTaken();
        }

        public Team Team((int x, int y) posObj)
        {
            return Cubes[posObj.x][posObj.y].Team();
        }

        public void Activate(ChessPiece chessPiece, Controller con, (int x, int y) posObj, Moves move)
        {
            // Debug.Log("Activate");
            Cubes[posObj.x][posObj.y].Activate(chessPiece, con, move);
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

        public Position GetPosition((int x, int y) movesMoveResultPos)
        {
            try
            {
                return Cubes[movesMoveResultPos.x][movesMoveResultPos.y];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log($"error with x={movesMoveResultPos.x} y={movesMoveResultPos.y}");
                throw;
            }
            
        }
        public Position GetPosition(string pos)
        {

            return Cubes[Position.String2Number(pos[0].ToString().ToUpper())][int.Parse(pos.Substring(1,pos.Length-1))-1];
        }

        public List<Position> GetPositions()
        {
            List<Position> list = new List<Position>();
            foreach (var positions in Cubes)
            {
                list.AddRange(positions);
            }
            return list;
        }

        public bool IsEnPassant(string s)
        {
            return CurrentBoardStateData.EnPassantSquare.ToLower() == s.ToLower();
        }

        public BoardStateData GetBoardStateData(Controller controller, Director director)
        {
            BoardToFenMapper mapper = new BoardToFenMapper(GetPositions(), controller, director);
            fenParser = new FenParser(mapper.GetMap());
            SetBoardStateData(fenParser.BoardStateData);
            return CurrentBoardStateData;
        }

        public void SetBoardStateData(BoardStateData fenParserBoardStateData)
        {
            foreach (List<Position> posList in Cubes)
            {
                foreach (Position pos in posList)
                {
                    pos.SetEnPassant(false);
                }
            }
            CurrentBoardStateData = fenParserBoardStateData;
            if (CurrentBoardStateData.EnPassantSquare.Length > 0)
            {
                var enPassant = GetPosition(CurrentBoardStateData.EnPassantSquare);
                enPassant.SetEnPassant(true);
            }
        }
    }
    
}