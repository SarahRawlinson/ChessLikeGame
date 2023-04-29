using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LibObjects;
using Multiplayer.Controllers;
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

    public ChessEngine getChessEngine()
    {
        return _chessEngine;
    }

    private void Awake()
    {
        ChessSquare.onSelectedSquareEvent += ChessSquareOnSelectedSquareEvent; 
        ChessSquare.onPossibleMoveSelected += ChessSquareOnPossibleMoveSelected;
        WebSocketConnection.onGameRoomMessageRecieved += WebSocketConnectionOnonGameRoomMessageRecieved;
    }

    private void WebSocketConnectionOnonGameRoomMessageRecieved((Room room, User user, string Message) obj)
    {
        if (!isHost)
        {
            
        }
    }

    private void ChessSquareOnPossibleMoveSelected(int obj)
    {
        foreach (var mv in _chessEngine.GetRules().GetMovesByPiece(_chessEngine.GetGameBoardList()[currentSelection].PieceOnGrid, currentSelection))
        {
            if (mv.EndPosition == obj)
            {
                MovePiece(mv);
                ClearChessSquarePossibleMoves();
                return;
            }
        }
        
        
     
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


    private void StartGame((Room gameRoom, User thisPlayer, User opponent, User host) obj)
    {
        gameRoom = obj.gameRoom;
        if (obj.thisPlayer.GetUserGuid() == obj.host.GetUserGuid())
        {
            isHost = true;
            isWhite = true;
        }
    
        _chessEngine.CreateBoard();
        _chessEngine.GetFenController().SetUpBoardFromFen(setupFenString);
        
        gameObjectController.CreateBoardPositions(8,8);

        int counter = 0;
         foreach (var square in _chessEngine.GetGameBoardList())
        {
            if (square.PieceOnGrid.GetPieceType() != ChessPieceTypes.NONE)
            {
                gameObjectsPieces.Add(square.GetKey(),gameObjectController.SpawnPiece(
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
        MovePiece(TestMove("H7"));
        MovePiece(TestMove("F7"));
        MovePiece(TestMove("G8"));
        MovePiece(TestMove("A2"));
        MovePiece(TestMove("B2"));
        MovePiece(TestMove("C2"));
        MovePiece(TestMove("D2"));
        MovePiece(TestMove("E2"));
        MovePiece(TestMove("F2"));
        MovePiece(TestMove("G2"));
        MovePiece(TestMove("H2"));
        MovePiece(TestMove("G8"));
        MovePiece(TestMove("B8"));
        MovePiece(TestMove("A7"));
        MovePiece(TestMove("B7"));
        MovePiece(TestMove("C7"));
        MovePiece(TestMove("D7"));
        MovePiece(TestMove("E7"));
        MovePiece(TestMove("F7"));
        MovePiece(TestMove("G7"));
        MovePiece(TestMove("H7"));
        

    }

    private void MovePiece(Move move)
    {
        if (move == null)
        {
            Debug.Log("FAILED: ATTEMPTED TO MOVE A NULL PIECE" );
            return;
        }
        
        Debug.Log("Attempting to move (in MovePiece) :" + move);
        if (_chessEngine.MakeMoveOnBoard(move))
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
            gameObjectController.MoveGameObject(
                gameObjectIndex,
                gameObjectController.GetPositionVectorfromGameSquare(move.EndPosition)
                );
            gameObjectsPieces[endGrid.GetKey()] = gameObjectIndex;
            // removed from game without breaking indexing of game objects?
            gameObjectsPieces[startGrid.GetKey()] = -1;
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
