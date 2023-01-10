using System;
using System.Collections.Generic;
using Chess.Board;
using Chess.Enums;
using Chess.Pieces;
using TMPro;
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
        private BoardObject _boardObject;
        public event Action OnStart;
        private Controller _activeController;
        private bool _gameOver = false;
        public Team team;
        [SerializeField] ChessPiece[] chessPieces;
        private Dictionary<string, string> boardSetUpWhite = new Dictionary<string, string>();
        private Dictionary<string, string> boardSetUpBlack = new Dictionary<string, string>();

        private void BuildDictionarySetUp()
        {
            boardSetUpWhite.Add("a1","rook");
            boardSetUpWhite.Add("b1","knight");
            boardSetUpWhite.Add("c1","bishop");
            boardSetUpWhite.Add("d1","queen");
            boardSetUpWhite.Add("e1","king");
            boardSetUpWhite.Add("f1","bishop");
            boardSetUpWhite.Add("g1","knight");
            boardSetUpWhite.Add("h1","rook");
            boardSetUpWhite.Add("a2","pawn");
            boardSetUpWhite.Add("b2","pawn");
            boardSetUpWhite.Add("c2","pawn");
            boardSetUpWhite.Add("d2","pawn");
            boardSetUpWhite.Add("e2","pawn");
            boardSetUpWhite.Add("f2","pawn");
            boardSetUpWhite.Add("g2","pawn");
            boardSetUpWhite.Add("h2","pawn");
            
            boardSetUpBlack.Add("a8","rook");
            boardSetUpBlack.Add("b8","knight");
            boardSetUpBlack.Add("c8","bishop");
            boardSetUpBlack.Add("d8","queen");
            boardSetUpBlack.Add("e8","king");
            boardSetUpBlack.Add("f8","bishop");
            boardSetUpBlack.Add("g8","knight");
            boardSetUpBlack.Add("h8","rook");
            boardSetUpBlack.Add("a7","pawn");
            boardSetUpBlack.Add("b7","pawn");
            boardSetUpBlack.Add("c7","pawn");
            boardSetUpBlack.Add("d7","pawn");
            boardSetUpBlack.Add("e7","pawn");
            boardSetUpBlack.Add("f7","pawn");
            boardSetUpBlack.Add("g7","pawn");
            boardSetUpBlack.Add("h7","pawn");

            foreach (var piece in boardSetUpWhite)
            {
                var obj = ChessPieceType(piece);

                var position = _boardObject.GetPosition(piece.Key);
                var gObj = position.SpawnPiece(obj.gameObject);
                ChessPiece chessPiece = gObj.GetComponent<ChessPiece>();
                chessPiece.team = Team.White;
                chessPiece.MeshRender.material = teamWhiteColour;
                var pos = position.GetPos();
                chessPiece.pos = new Vector2(pos.x, pos.y);
            }
            foreach (var piece in boardSetUpBlack)
            {
                var obj = ChessPieceType(piece);

                var position = _boardObject.GetPosition(piece.Key);
                var gObj = position.SpawnPiece(obj.gameObject);
                ChessPiece chessPiece = gObj.GetComponent<ChessPiece>();
                chessPiece.team = Team.Black;
                chessPiece.MeshRender.material = teamBlackColour;
                var pos = position.GetPos();
                chessPiece.pos = new Vector2(pos.x, pos.y);
            }
            
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
            // if (Input.GetKey("escape"))
            // {
            //     Application.Quit();
            // }
        }

        private void Awake()
        {
            ChessPiece.OnSwap += SwapPiece;
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
            StartGame();
        }

        private void Start()
        {
            _boardObject = FindObjectOfType<BoardObject>();
            // BuildDictionarySetUp();
            StartGame();
            King.OnEnd += End;
            ChessPiece.TeamSwitch += SetColours;
            _boardObject.SetReady();
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

        private void StartGame()
        {
            player1.SetTeam(Team.Black);
            player1.OnMoved += MoveMade;
            player2.SetTeam(Team.White);
            OnStart?.Invoke();
            player1.SetActive();
        }

        private void MoveMade(Controller controller)
        {
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
            
            
        }
    }
}