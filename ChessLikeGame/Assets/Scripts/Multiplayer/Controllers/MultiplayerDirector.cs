using System;
using System.Collections.Generic;
using Chess.Fen;
using Chess.Pieces;
using Multiplayer;
using Multiplayer.Controllers;
using Multiplayer.Models;
using Multiplayer.Models.Movement;
using Multiplayer.Models.Rules;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(GameObjectController))]
public class MultiplayerDirector : MonoBehaviour
{
    private bool _gameOver = false;

    private MultiGameStateData _gameStateData;

    private string setupFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private GameObjectController gameObjectController;
    private Rules _rules;
    
    public bool SetupNewGame()
    {
        int counter = 0;

        
        _gameStateData = new MultiGameStateData(setupFenString);
        Debug.Log(setupFenString);
        var gameBoardList = _gameStateData.GetGameBoardList();


        gameObjectController.CreateBoardPositions(8,8);

         counter = 0;

         foreach (var square in gameBoardList)
        {
            if (square.pieceOnGrid.GetType() != ChessPieceTypes.NONE)
            {
                gameObjectController.SpawnPiece(
                    square.pieceOnGrid.GetType(), 
                    square.pieceOnGrid.Colour,
                    gameObjectController.GetPositionVectorfromGameSquare(counter)
                );
            }

            counter++;
        }

         _rules = new Rules(_gameStateData);
        
        return true;
    }

    private void MovePiece()
    {
        Move move = _rules.GetMovesByPiece(_gameStateData.GetGameBoardList()[ChessGrid.GetIndexFromKey("B2")]
            .pieceOnGrid)[0];
        if (_gameStateData.MakeMoveOnBoard(move))
        {
            gameObjectController.MoveGameObject()
        }
        
    }

    private void Start()
    {
        gameObjectController = gameObject.GetComponent<GameObjectController>();
        MovePiece();
    }
}
