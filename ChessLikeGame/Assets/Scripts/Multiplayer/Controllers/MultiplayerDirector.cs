using System;
using System.Collections.Generic;

using Multiplayer;
using Multiplayer.Controllers;
using Multiplayer.Models;
using Multiplayer.Models.Movement;
using Multiplayer.Models.Rules;
using UnityEngine;


[RequireComponent(typeof(GameObjectController))]
public class MultiplayerDirector : MonoBehaviour
{
    private bool _gameOver = false;

    private MultiGameStateData _gameStateData;
    private Dictionary<string, int> gameObjectsPieces = new Dictionary<string, int>();

    private string setupFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private GameObjectController gameObjectController;
    private Rules _rules;
    
    public bool SetupNewGame()
    {
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
         
        MovePiece();
        return true;
    }

    private void MovePiece()
    {
        int fromKey = ChessGrid.GetIndexFromKey("G1");
        ChessGrid startGrid = _gameStateData.GetGameBoardList()[fromKey];
        (int x, int y) = ChessGrid.CalculateXYFromIndex(fromKey);
        Debug.Log($"index: {fromKey}, XY: ({x},{y}), key: {startGrid.GetKey()}, piece: {startGrid.PieceOnGrid.GetPieceType()}, piece XY: ({startGrid.PieceOnGrid.X},{startGrid.PieceOnGrid.Y})");
        var movesByPiece = _rules.GetMovesByPiece(startGrid
            .PieceOnGrid);
        
        Move move = movesByPiece[0];
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

    private void Start()
    {
        gameObjectController = gameObject.GetComponent<GameObjectController>();
        
    }
}
