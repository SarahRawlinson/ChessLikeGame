using System;
using System.Collections.Generic;
using Chess.Sound;
using JetBrains.Annotations;
using LibObjects;
using Multiplayer.Controllers;
using Multiplayer.Models;
using Multiplayer.Models.BoardState;
using Multiplayer.Models.Movement;
using Multiplayer.View.LoadData;
using UnityEngine;

public class MultiplayerDirector : MonoBehaviour
{
    private bool _gameOver = false;

    private Dictionary<string, int> gameObjectsPieces = new Dictionary<string, int>();
    private string setupFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private GameObjectController gameObjectController;
    private Room gameRoom;
    private bool isWhite = false;

    private ChessEngine _chessEngine = new ChessEngine();
    private bool isHost = false;
    private int currentSelection;
    private WebSocketConnection _connection;
    private SoundEffects _soundEffects;
    [SerializeField] private float pieceMovementSpeed = 1.25f;
    [SerializeField] private float pieceYMaxHeight = 0.25f;
    public static event Action NetGameStarted;   

    public ChessEngine getChessEngine()
    {
        return _chessEngine;
    }

    private void Awake()
    {
        _connection = FindObjectOfType<WebSocketConnection>();
        _soundEffects = FindObjectOfType<SoundEffects>();
        ChessSquare.onSelectedSquareEvent += ChessSquareOnSelectedSquareEvent; 
        ChessSquare.onPossibleMoveSelected += ChessSquareOnPossibleMoveSelected;
        WebSocketConnection.onGameRoomMessageRecieved += WebSocketConnectionOnonGameRoomMessageRecieved;
    }

    private void WebSocketConnectionOnonGameRoomMessageRecieved((Room room, User user, string Message) obj)
    {
        gameRoom = obj.room;
        var MessageArray = obj.Message.Split(':',StringSplitOptions.None);

        switch (MessageArray[0])
        { case "FEN_Setup":
                ProcessBoardSetup(MessageArray[1]);
                break;
            case "Request_To_Move":
                ProcessMoveRequest(MessageArray[1]);
                break;
            case "Request_Rejected":
                ProcessRejected(MessageArray[1]);
                break;
            case "Make_Move":
                ProcessNetworkMove(MessageArray[1], MessageArray[2]);
                break;
            default:
                Console.WriteLine("UNEXPECTED MESSAGE: " + MessageArray[0]);
                break;
        }

        void ProcessRejected(string data)
        {
            if (!isHost)
            {
                Debug.Log("SERVER SENT REJECTION:"+ data);
            }
        }

        void ProcessMoveRequest(string data)
        {
            if (isHost)
            {
                Move testMove = new Move(data);
                if (_chessEngine.GetRules().isMoveValid(testMove))
                {
                    MovePiece(testMove);
                    SendMakeMoveMessage(testMove);
                }
                else
                {
                    string reason = "Move Failed Validation!";
                    _connection.SendMessageToRoom(gameRoom, "Request_Rejected:" + reason);
                }

            }
        }

        void ProcessBoardSetup(string data)
        { 
            if (!isHost){
                InitBoard(data);
                StartGame((gameRoom, _connection.GetClientUser(), obj.user, obj.user));
                FindObjectOfType<StartGameScreenUI>().HideStartScreen();
            }
            
        }

        void ProcessNetworkMove(string data, string currentTurn)
        {
            if (!isHost)
            {
               // _chessEngine.SetActivePlayerColor(Enum.Parse<TeamColor>(currentTurn));
                MovePiece(new Move(data));
            }
        }
    }

    private void ChessSquareOnPossibleMoveSelected(int obj)
    {
  
        foreach (var mv in _chessEngine.GetRules().GetMovesByPiece(_chessEngine.GetGameBoardList()[currentSelection].PieceOnGrid, currentSelection))
        {
            if (mv.EndPosition == obj)
            {
                Debug.Log("Sending Move:"+ mv.GetNetworkData());
                Debug.Log("In GameRoom:" + gameRoom.GetGuid().ToString());
                if (isHost)
                {
                    if (isWhite == true && _chessEngine.GetActivePlayer() == TeamColor.White ||
                        isWhite == false && _chessEngine.GetActivePlayer() == TeamColor.Black)
                    {
                        MovePiece(mv);
                        SendMakeMoveMessage(mv);
                    }
                }
                else
                {
                    if (isWhite == true && _chessEngine.GetActivePlayer() == TeamColor.White ||
                        isWhite == false && _chessEngine.GetActivePlayer() == TeamColor.Black)
                    {
                        _connection.SendMessageToRoom(gameRoom, "Request_To_Move:" + mv.GetNetworkData());
                    }
                }

                ClearChessSquarePossibleMoves();
                return;
            }
        }
        
        
     
    }

    private void SendMakeMoveMessage(Move mv)
    {
        _connection.SendMessageToRoom(gameRoom, "Make_Move:" + mv.GetNetworkData()+ ":" + _chessEngine.GetActivePlayer());
    }

    private void ChessSquareOnSelectedSquareEvent(int obj)
    {
        currentSelection = obj;
        ClearChessSquarePossibleMoves();
        foreach (var mv in _chessEngine.GetRules().GetMovesByPiece(_chessEngine.GetGameBoardList()[obj].PieceOnGrid, obj))
        {
            gameObjectController.GetChessCube(mv.EndPosition).SetPossibleMove(true);
        }
        
    }

    private void ClearChessSquarePossibleMoves()
    {
        for (int a = 0; a < 64; a++)
        {
            gameObjectController.GetChessCube(a).SetPossibleMove(false);
        }
    }


    private void StartGame((Room gameRoom, User thisPlayer, User oponent, User host) obj)
    {
        Debug.Log("Starting New Game.. Server?:" + isHost);
        gameRoom = obj.gameRoom;
        if (obj.thisPlayer.GetUserGuid() == obj.host.GetUserGuid())
        {
            isHost = true;
            isWhite = true;
        }

        if (isHost)
        {
            InitBoard(setupFenString);
            _connection.SendMessageToRoom(obj.gameRoom, "FEN_Setup:"+ _chessEngine.GetFenController().FenBuilder());
            
        }
        NetGameStarted?.Invoke();
    }

    private void InitBoard(string startingFENString)
    {
        _chessEngine.CreateBoard();
        _chessEngine.GetFenController().SetUpBoardFromFen(startingFENString);

        gameObjectController.CreateBoardPositions(8, 8);

        int counter = 0;
        foreach (var square in _chessEngine.GetGameBoardList())
        {
            if (square.PieceOnGrid.GetPieceType() != ChessPieceTypes.NONE)
            {
                gameObjectsPieces.Add(square.GetKey(), gameObjectController.SpawnPiece(
                    square.PieceOnGrid.GetPieceType(),
                    square.PieceOnGrid.Colour,
                    gameObjectController.GetPositionVectorfromGameSquare(counter)
                ));
            }
            else
            {
                gameObjectsPieces.Add(square.GetKey(), -1);
            }

            counter++;
        }
    }

    private void MovePiece(Move move)
    {
        if (move == null)
        {
            Debug.Log("FAILED: ATTEMPTED TO MOVE A NULL PIECE" );
            return;
        }
        
        Debug.Log("Attempting to move (in MovePiece) :" + move);
        if (_chessEngine.MakeMoveOnBoard(ref move))
        {
            Debug.Log("GameBoard Data Array update with move (in MovePiece) :" + move);
            
            ChessGrid endGrid = _chessEngine.GetGameBoardList()[move.EndPosition];
            ChessGrid startGrid = _chessEngine.GetGameBoardList()[move.StartPosition];
            //checking for null position
            if (gameObjectsPieces[endGrid.GetKey()] != -1)
            {
                gameObjectController.RemoveGameObjectFromGame(gameObjectsPieces[endGrid.GetKey()]);
            }
            int gameObjectIndex = gameObjectsPieces[startGrid.GetKey()];
            StartCoroutine(gameObjectController.MoveGameObject(
                gameObjectIndex,
                gameObjectController.GetPositionVectorfromGameSquare(move.EndPosition)
               ,pieceMovementSpeed, pieceYMaxHeight ));
            gameObjectsPieces[endGrid.GetKey()] = gameObjectIndex;
            // removed from game without breaking indexing of game objects?
            gameObjectsPieces[startGrid.GetKey()] = -1;
            ClearChessSquarePossibleMoves();
            if(move.WillResultInCapture){
                _soundEffects.PlayChessPieceCaptureSound();
            }
            else
            {
                _soundEffects.PlayChessPieceDownSound();
            }
        }
    }


    [CanBeNull]
    private Move TestMove(string gridKey)
    {
        int gridIndex = ChessGrid.GetIndexFromKey(gridKey);
        
        var testMoves = _chessEngine.GetRules().GetMovesByPiece(_chessEngine.GetGameBoardList()[gridIndex].PieceOnGrid, gridIndex);
        Debug.Log(
            $"index: {gridIndex}, key: {gridKey}, piece: {_chessEngine.GetGameBoardList()[gridIndex].PieceOnGrid.GetPieceType()})"); 

        if (testMoves.Count == 0)
        {
            Debug.Log($"cant move {_chessEngine.GetGameBoardList()[gridIndex].PieceOnGrid.GetPieceType()}");
            return null;
        }

        return testMoves[0];

    }

    private void Start()
    {
        gameObjectController = FindObjectOfType<GameObjectController>();
        StartGameScreenUI.onGameStarted += StartGame;
    }
}
