using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Chess.Board;
using Chess.Enums;
using Chess.Fen;
using Chess.Movement;
using Chess.Pieces;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static Chess.Fen.FenExamples;

namespace Chess.Control
{
    public class Director: MonoBehaviour
    {
        [FormerlySerializedAs("player1")] [SerializeField] private Controller blackPlayer;
        [FormerlySerializedAs("player2")] [SerializeField] private Controller whitePlayer;
        [SerializeField] private TMP_Text endText;
        [SerializeField] private Material teamBlackColour;
        [SerializeField] private Material teamWhiteColour;
        [SerializeField] private string fenStartString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        [SerializeField] bool useFenStringEnum = true;
        [SerializeField] public FenStringsEnum fenStringEnumEnum = FenStringsEnum.StartPosition;
        private BoardObject _boardObject;
        public event Action OnStart;
        private Controller _activeController;
        private bool _gameOver = false;
        public Team team;
        [SerializeField] ChessPiece[] chessPieces;
        private CreateBoardFromFen boardMaker;
        public int FullMove { get; set; }
        public int HalfmoveClock { get; set; }
        public int TurnNumber { get; set; }
        public static event Action OnMoveMade;
        private King blackKing;
        private King whiteKing;
        private int counter = 0;
        
        private bool BoardSetUp = false;


        public GameObject CreatePiece(KeyValuePair<string, string> piece, Team chessPieceTeam)
        {
            //TODO
            if (BoardSetUp)
            {
                Debug.Log("needs implementing");
                throw CheckoutException.Canceled;
            }

            var obj = ChessPieceType(piece);

            var position = _boardObject.GetPosition(piece.Key);
            var gObj = position.SpawnPiece(obj.gameObject);
            ChessPiece chessPiece = gObj.GetComponent<ChessPiece>();
            position.SetPiece(chessPiece);
            chessPiece.team = chessPieceTeam;
            chessPiece.MeshRender.material = chessPieceTeam==Team.Black?teamBlackColour:teamWhiteColour;
            var pos = position.GetPos();
            chessPiece.pos = new Vector2(pos.x, pos.y);
            if (chessPiece.TryGetComponent(out King king))
            {
                if (chessPieceTeam == Team.Black)
                {
                    blackKing = king;
                }
                else
                {
                    whiteKing = king;
                }
            }
            return gObj;
        }

        private ChessPiece ChessPieceType(KeyValuePair<string, string> piece)
        {
            var obj = chessPieces[0]; 
            switch (piece.Value)
            {
                case "pawn":
                    obj = chessPieces[3];
                    break;
                case "rook":
                    obj = chessPieces[5];
                    break;
                case "knight":
                    obj = chessPieces[2];
                    break;
                case "bishop":
                    obj = chessPieces[0];
                    break;
                case "queen":
                    obj = chessPieces[4];
                    break;
                case "king":
                    obj = chessPieces[1];
                    break;
                default:
                    break;
            }

            return obj;
        }

        private void Update()
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }

        private void Awake()
        {
            ChessPiece.OnSwap += SwapPiece;
            ChessPiece.OnTakenStatic += ResetHalfMoveClock;
            Pawn.OnMoveStatic += ResetHalfMoveClock;
        }

        private void ResetHalfMoveClock()
        {
            HalfmoveClock = 0;
        }

        private void SwapPiece(ChessPiece arg1, int arg2)
        {
            int i = UnityEngine.Random.Range(0, chessPieces.Length);
            arg1.DeactivateGameObject();
            Position pos = _boardObject.GetPosition(arg1.GetPositionXY());
            ChessPiece obj = Instantiate(chessPieces[i], arg1.transform.parent);
            obj.transform.position = arg1.transform.position; 
            obj.SetUpCopy(arg1);
        }

        public Material GetTeamMaterial(Team team)
        {
            if (team == Team.Black)
            {
                return teamBlackColour;
            }
            return teamWhiteColour;
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void NewGame()
        {
            boardMaker = new CreateBoardFromFen(this);
            StartGame(boardMaker.fenParser.BoardStateData);
        }

        private void Start()
        {
            _boardObject = FindObjectOfType<BoardObject>();
            boardMaker = new CreateBoardFromFen(this);
            if (useFenStringEnum)
            {
                fenStartString = GetFenByName(fenStringEnumEnum);
            }
            boardMaker.SetUpBoardFromFen(fenStartString);
            StartGame(boardMaker.fenParser.BoardStateData);
            King.OnEnd += End;
            ChessPiece.TeamSwitch += SetColours;
            _boardObject.SetReady();
            BoardSetUp = true;
        }

        private void SetColours(GameObject arg1, Team arg2)
        {
            arg1.GetComponent<MeshRenderer>().material = GetTeamMaterial(arg2);
        }


        private void End(Team obj)
        {
            _gameOver = true;
            string s = obj == Team.Black ? "White" : "Black";
            ShowEndText(true, s);
        }

        void ShowEndText(bool on, string s = "")
        {
            endText.enabled = on;
            if (on) endText.text = $"Game Over Team {s} Won";
        }

        private void StartGame(BoardStateData boardState)
        {
            _boardObject.SetBoardStateData(boardState);
            HalfmoveClock = boardState.HalfMoveCounter;
            FullMove = boardState.FullMoveNumber;
            TurnNumber = boardState.FullMoveNumber * 2 + (boardState.ActivePlayerColor == "White"?0:1);
            blackPlayer.SetTeam(Team.Black);
            whitePlayer.SetTeam(Team.White);
            Controller player = boardState.ActivePlayerColor == "White" ? whitePlayer : blackPlayer;
            _activeController = player;
            player.OnMoved += MoveMade;
            OnStart?.Invoke();
            player.SetActive();
            SetUpForPlayer(player);
        }
        
        private void MoveMade(Controller controller)
        {
            if (IsCheck())
            {
                Debug.Log("Check!!!");
            }
            if (blackPlayer._team == controller._team)
            {
                blackPlayer.OnMoved -= MoveMade;
                whitePlayer.OnMoved += MoveMade;
                _activeController = whitePlayer;
                whitePlayer.SetActive();
                // Debug.Log($"Active controller is now {player2}");
            }
            else
            {
                whitePlayer.OnMoved -= MoveMade;
                blackPlayer.OnMoved += MoveMade;
                _activeController = blackPlayer;
                blackPlayer.SetActive();
                // Debug.Log($"Active controller is now {player1}");
            }
            
            team = _activeController._team;
            
            HalfmoveClock++;
            TurnNumber++;
            if (controller._team == Team.Black) FullMove++;
            OnMoveMade?.Invoke();
            if (SetUpForPlayer(controller)) return;
        }

        private bool SetUpForPlayer(Controller controller)
        {
            counter++;
            Debug.Log(counter);
            
            _boardObject.ClearBoard();
            Controller opponent = controller == blackPlayer ? whitePlayer : blackPlayer;
            if (_gameOver || PlayerHasOpponentInCheckMate(opponent, controller._king))
            {
                Debug.Log("Game Over");
                return true;
            }

            var king = controller.GetTeam() == Team.Black ? whiteKing : blackKing;
            if (PlayerHasOpponentInCheck(controller, king.GetPositionXY()))
            {
                king.SetInCheck(true);
            }
            else
            {
                king.SetInCheck(false);
            }
            ReMapFen(controller);

            return false;
        }

        private void ReMapFen(Controller controller)
        {
            BoardStateData data = _boardObject.GetBoardStateData(controller,this);
            blackKing.CanCastleKing = data.BlackCanKingsideCastle;
            whiteKing.CanCastleKing = data.WhiteCanKingsideCastle;
            blackKing.CanCastleQueen = data.BlackCanQueensideCastle;
            whiteKing.CanCastleQueen = data.WhiteCanQueensideCastle;
            Debug.Log(data.Fen);
        }

        public bool IsCheck()
        {
            if (PlayerHasOpponentInCheck(whitePlayer, blackKing.GetPositionXY(), true)) return true;
            if (PlayerHasOpponentInCheck(blackPlayer, whiteKing.GetPositionXY(), true)) return true;
            return false;
        }

        public bool PlayerHasOpponentInCheck(Controller player, (int x, int y) kingPosition, out string log)
        {
            log = "";
            foreach (var piece in player.pieces)
            {
                foreach (Moves move in piece.GetPossibleMoves())
                {
                    
                    if (move.MoveResultPos == kingPosition)
                    {
                        log = $"check! {piece.team.ToString()} {piece.NameType} {piece.GetPosition().GetCoordinates()} can take king";
                        return true;
                    }
                }
            }
            return false;
        }
        public bool PlayerHasOpponentInCheck(Controller player, (int x, int y) kingPosition, bool print = false)
        {
            bool on = PlayerHasOpponentInCheck(player, kingPosition, out string log);
            if (on && print) Debug.Log(log);
            return on;
        }
        
        public bool PlayerHasOpponentInCheck(Controller player, (int x, int y) kingPosition, out string log, out List<Moves> checkMoves)
        {
            log = "";
            checkMoves = new List<Moves>();
            foreach (var piece in player.pieces)
            {
                foreach (Moves move in piece.GetPossibleMoves())
                {
                    
                    if (move.MoveResultPos == kingPosition)
                    {
                        log = $"check! {piece.team.ToString()} {piece.NameType} {piece.GetPosition().GetCoordinates()} can take king";
                        checkMoves.Add(move);
                    }
                }
            }
            return checkMoves.Count>0;
        }
        
        public bool PlayerHasOpponentInCheckMate(Controller player, King king)
        {
            StringBuilder checkMate = new StringBuilder();
            Controller opponent = player == blackPlayer ? whitePlayer : blackPlayer;
            List<Moves> possibleMoves = king.GetPossibleMoves();
            Dictionary<(int x, int y), List<Moves>> checkPositions = new Dictionary<(int x, int y), List<Moves>>();
            if (possibleMoves.Count == 0)
            {
                if (!PlayerHasOpponentInCheck(player, king.GetPositionXY(), out string log, out List<Moves> moves)) return false;
                checkMate.Append($"{log}\n");
                checkPositions.Add(king.GetPositionXY(),moves);
            }
            else
            {
                foreach (Moves move in possibleMoves)
                {
                    if (!PlayerHasOpponentInCheck(player, move.MoveResultPos, out string log, out List<Moves> moves)) return false;
                    checkPositions.Add(move.MoveResultPos, moves);
                    checkMate.Append($"{log}\n");
                }
            }
            //TODO check this is correct
            //can any of the players pieces counter attack?
            foreach (var position in checkPositions)
            {
                if (position.Value.Count > 1) continue;
                var move = position.Value[0];
                
                foreach (var piece in opponent.pieces)
                {
                    foreach (var counterMove in piece.GetPossibleMoves())
                    {
                        // can piece take?
                        if (counterMove.MoveResultPos == move.Piece.GetPositionXY())
                        {
                            return false;
                        }
                        //can piece get in way
                        var cMY = counterMove.MoveResultPos.y;
                        var cMX = counterMove.MoveResultPos.x;
                        var mY = move.MoveResultPos.y;
                        var mX = move.MoveResultPos.x;
                        var pX = move.Piece.GetPositionXY().x;
                        var pY = move.Piece.GetPositionXY().y;
                        bool betweenHorizontal = cMY > pY && cMY < mY || cMY < pY && cMY > mY;
                        bool betweenVertical = cMX > pX && cMX < mX || cMX < pX && cMX > mX;

                        bool sameDiagonal = false;
                        switch (move.MoveType)
                        {
                            case MoveTypes.L:
                                break;
                            case MoveTypes.Backward or MoveTypes.Forward:
                                if (cMY != mY) continue;
                                if (betweenHorizontal) return false;
                                break;
                            case MoveTypes.Left or MoveTypes.Right:
                                if (cMX != mX) continue;
                                if (betweenVertical) return false;
                                break;
                            case MoveTypes.DiagonalDownLeft or MoveTypes.DiagonalUpRight or MoveTypes.DiagonalDownRight or MoveTypes.DiagonalUpLeft:
                                int differanceVerticalCounterMove = Mathf.Abs(cMX - pX);
                                int differanceHorizontalCounterMove = Mathf.Abs(cMY - pY);
                                if (betweenHorizontal && betweenVertical &&
                                    differanceHorizontalCounterMove == differanceVerticalCounterMove) return false;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            
            Debug.Log($"Check Mate! \n{checkMate}");
            return true;
        }
        
        public bool IsCheck(Team team, (int x, int y) position)
        {
            Controller player = team == Team.White ? blackPlayer : whitePlayer;
            bool on = PlayerHasOpponentInCheck(player, position, out string log);
            return on;
        }
    }
}