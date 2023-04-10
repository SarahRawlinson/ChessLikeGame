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

    private MultiGameStateData _gameStateData;
    private Dictionary<string, int> gameObjectsPieces = new Dictionary<string, int>();

    private string setupFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private GameObjectController gameObjectController;
    private Rules _rules;
    private Room gameRoom;
    private bool isHost = false;

    private void StartGame((Room gameRoom, User thisPlayer, User opponent, User host) obj)
    {
        gameRoom = obj.gameRoom;
        if (obj.thisPlayer.GetUserGuid() == obj.host.GetUserGuid())
        {
            isHost = true;
        }
        int counter = 0;
        _gameStateData = new MultiGameStateData(setupFenString);
        _rules = new Rules(_gameStateData);
        var gameBoardList = _gameStateData.GetGameBoardList();
        gameObjectController.CreateBoardPositions(8,8);
        counter = 0;

         foreach (var square in gameBoardList)
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
         
        if (TestMove(out var startGrid, out var move)) return;
        MovePiece(startGrid, move);
    }

    private void MovePiece(ChessGrid startGrid, Move move)
    {
        
        if (_gameStateData.MakeMoveOnBoard(move))
        {
            ChessGrid endGrid = _gameStateData.GetGameBoardList()[move.EndPosition];
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
            gameObjectsPieces[startGrid.GetKey()] = -1;
        }
    }

    private bool TestMove(out ChessGrid startGrid, out Move move)
    {
        move = new Move(MoveTypes.Backward, 0, 0, TeamColor.Black);
        int fromKey = ChessGrid.GetIndexFromKey("H8");
        startGrid = _gameStateData.GetGameBoardList()[fromKey];
        (int x, int y) = ChessGrid.CalculateXYFromIndex(fromKey);
        Debug.Log(
            $"index: {fromKey}, XY: ({x},{y}), key: {startGrid.GetKey()}, piece: {startGrid.PieceOnGrid.GetPieceType()}, piece XY: ({startGrid.PieceOnGrid.X},{startGrid.PieceOnGrid.Y})");
        var movesByPiece = _rules.GetMovesByPiece(startGrid
            .PieceOnGrid);
        if (movesByPiece.Count == 0)
        {
            Debug.Log($"cant move {startGrid.PieceOnGrid.GetPieceType()}");
            return true;
        }

        move = movesByPiece[0];
        return false;
    }

    private void Start()
    {
        gameObjectController = FindObjectOfType<GameObjectController>();
        StartGameScreenUI.onGameStarted += StartGame;
    }
}
