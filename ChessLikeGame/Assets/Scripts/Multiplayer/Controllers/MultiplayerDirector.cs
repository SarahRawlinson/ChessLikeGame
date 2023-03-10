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
    private List<ChessGrid> gameBoard = new List<ChessGrid>();
    private TeamColor playerToMove;
    private string setupFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private GameObjectController gameObjectController;
    
    public bool SetupNewGame()
    {
        int counter = 0;
        gameBoard.Clear();
        

        for(; counter < 64; counter++)
        {
            ChessGrid chessGrid = new ChessGrid(new MultiPiece(), counter);
            Debug.Log($"creating board position, index:{gameBoard.Count}, position: {chessGrid.GetKey()}");
            gameBoard.Add(chessGrid);
        }
        
        
        LoadPositionsFromFENString(setupFenString);
        
        playerToMove = TeamColor.White;
        gameObjectController.CreateBoardPositions(8,8);

         counter = 0; 
        foreach (var square in gameBoard)
        {
            if (square.pieceOnGrid.GetType() != ChessPieceTypes.NONE)
            {
                gameObjectController.SpawnPiece(square.pieceOnGrid.GetType(), square.pieceOnGrid.Colour,gameObjectController.GetPositionVectorfromGameSquare(counter)) ;
            }

            counter++;
        }
        
        return true;
    }


    public void LoadPositionsFromFENString(string inputFEN)
    {

        var processed = inputFEN.Split(" ");
        int counter = 0;
        foreach (var character in processed[0])
        {
            MultiPiece tmpPiece = new MultiPiece(); 
            
            switch (character)
            {
                case 'P':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.SetType(ChessPieceTypes.PAWN);
                    break;
                case 'R':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.SetType(ChessPieceTypes.ROOK);
                    break;
                case 'N':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.SetType(ChessPieceTypes.KNIGHT);
                    break;
                case 'B':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.SetType(ChessPieceTypes.BISHOP);
                    break;
                case 'Q':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.SetType(ChessPieceTypes.QUEEN);
                    break;
                case 'K':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.SetType(ChessPieceTypes.KING);
                    break;
                case 'p':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.SetType(ChessPieceTypes.PAWN);
                    break;
                case 'r':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.SetType(ChessPieceTypes.ROOK);
                    break;
                case 'n':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.SetType(ChessPieceTypes.KNIGHT);
                    break;
                case 'b':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.SetType(ChessPieceTypes.BISHOP);
                    break;
                case 'q':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.SetType(ChessPieceTypes.QUEEN);
                    break;
                case 'k':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.SetType(ChessPieceTypes.KING);
                    break;
                default:
                    break;
            }
            
            Debug.Log("Adding Piece to Board:" + tmpPiece.GetType() + ":" + tmpPiece.Colour + " / Counter:" + counter);

            gameBoard[counter].pieceOnGrid = tmpPiece;

            if (Char.IsDigit(character))
            {
              counter += (int) Char.GetNumericValue(character) -1;
            }

            if(character != '/'){
                counter++;
            }
        }
        
        switch (processed[1]) {
            case "w":
                playerToMove = TeamColor.White;
                break;
            case "b" :
                playerToMove = TeamColor.Black;
                break;
        }
    }

    private void Start()
    {
        gameObjectController = gameObject.GetComponent<GameObjectController>();
    }
}
