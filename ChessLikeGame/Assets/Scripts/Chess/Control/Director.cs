using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Chess.Board;
using Chess.Enums;
using Chess.Fen;
using Chess.Pieces;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Chess.Control
{
    public class Director: MonoBehaviour
    {
        [SerializeField] private Controller player1;
        [SerializeField] private Controller player2;
        [SerializeField] private TMP_Text endText;
        [SerializeField] private Material teamBlackColour;
        [SerializeField] private Material teamWhiteColour;
        [SerializeField] private string fenStartString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        private BoardObject _boardObject;
        public event Action OnStart;
        private Controller _activeController;
        private bool _gameOver = false;
        public Team team;
        [SerializeField] ChessPiece[] chessPieces;
        private CreateBoardFromFen boardMaker;
        public int FullMove {get => (TurnNumber)/2;}
        public int HalfmoveClock { get; set; }
        public int TurnNumber { get; set; }
        public static event Action OnMoveMade;
        private King blackKing;
        private King whiteKing;
        
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
            // BuildDictionarySetUp();
            boardMaker = new CreateBoardFromFen(this);
            boardMaker.SetUpBoardFromFen(fenStartString);
            _boardObject.SetBoardStateData(boardMaker.fenParser.BoardStateData);
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
            player1.SetTeam(Team.Black);
            player2.SetTeam(Team.White);

            Controller player = boardState.ActivePlayerColor == "White" ? player2 : player1;
            player.OnMoved += MoveMade;
            OnStart?.Invoke();
            player.SetActive();
        }
        
        private void MoveMade(Controller controller)
        {
            TurnNumber++;
            
            _boardObject.ClearBoard();
            if (_gameOver)
            {
                Debug.Log("Game Over");
                return;
            }
            if (player1._team == controller._team)
            {
                player1.OnMoved -= MoveMade;
                player2.OnMoved += MoveMade;
                _activeController = player2;
                player2.SetActive();
                // Debug.Log($"Active controller is now {player2}");
            }
            else
            {
                player2.OnMoved -= MoveMade;
                player1.OnMoved += MoveMade;
                _activeController = player1;
                player1.SetActive();
                // Debug.Log($"Active controller is now {player1}");
            }
            
            team = _activeController._team;
            ReMapFen(_activeController);
            HalfmoveClock++;
            OnMoveMade?.Invoke();
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
    }
}