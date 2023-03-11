using System;
using System.Collections.Generic;
using Chess.Fen;
using Chess.Pieces;
using Multiplayer;
using Multiplayer.Controllers;
using Multiplayer.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(GameObjectController))]
public class MultiplayerDirector : MonoBehaviour
{
    private bool _gameOver = false;

    private MultiBoardStateData _boardStateData;

    private string setupFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private GameObjectController gameObjectController;
    
    public bool SetupNewGame()
    {
        int counter = 0;

        
        _boardStateData = new MultiBoardStateData(setupFenString);
        Debug.Log(setupFenString);
        var gameBoardList = _boardStateData.GetGameBoardList();


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
        
        return true;
    }
    

    private void Start()
    {
        gameObjectController = gameObject.GetComponent<GameObjectController>();
    }
}
