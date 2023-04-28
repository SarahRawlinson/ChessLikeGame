using System;
using System.Collections.Generic;
using LibObjects;
using Multiplayer;
using Multiplayer.Controllers;
using Multiplayer.Models;
using Multiplayer.Models.BoardState;
using Multiplayer.Models.Movement;
using Multiplayer.Models.Rules;
using Multiplayer.View.LoadData;
using UnityEngine;

public class MultiplayerDirector : MonoBehaviour
{
    private bool _gameOver = false;

    private Dictionary<string, int> gameObjectsPieces = new Dictionary<string, int>();
    private string setupFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private GameObjectController gameObjectController;
    private Room gameRoom;

    private ChessEngine _chessEngine = new ChessEngine();
    private bool isHost = false;

    private void StartGame((Room gameRoom, User thisPlayer, User opponent, User host) obj)
    {
        gameRoom = obj.gameRoom;
        if (obj.thisPlayer.GetUserGuid() == obj.host.GetUserGuid())
        {
            isHost = true;
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
       
         
        if (!TestMove(out var move)) return;
        
        MovePiece(move);
    }

    private void MovePiece(Move move)
    {
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

    private bool TestMove(out Move move)
    {
        move = new Move(MoveTypes.Backward, 0, 0, TeamColor.Black);
        int fromKey = ChessGrid.GetIndexFromKey("B2");
        var startGrid = _chessEngine.GetGameBoardList()[fromKey];
        (int x, int y) = ChessGrid.CalculateXYFromIndex(fromKey);
        Debug.Log(
            $"index: {fromKey}, XY: ({x},{y}), key: {startGrid.GetKey()}, piece: {startGrid.PieceOnGrid.GetPieceType()})");
        var movesByPiece = _chessEngine.GetRules().GetMovesByPiece(startGrid.PieceOnGrid, fromKey);
        if (movesByPiece.Count == 0)
        {
            Debug.Log($"cant move {startGrid.PieceOnGrid.GetPieceType()}");
            return false;
        }

        move = movesByPiece[0];
        
        return true;
    }

    private void Start()
    {
        gameObjectController = FindObjectOfType<GameObjectController>();
        StartGameScreenUI.onGameStarted += StartGame;
    }
}
