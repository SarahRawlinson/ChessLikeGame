using System.Collections.Generic;
using Chess.Fen;
using Chess.Pieces;
using Multiplayer;
using Multiplayer.Models;
using Unity.VisualScripting;
using UnityEngine;

public class MultiplayerDirector : MonoBehaviour
{
    private bool _gameOver = false;
    private List<ChessGrid> gameBoard = new();
    private TeamColor playerToMove;
    private string setupFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private bool SetupNewGame()
    {
        int counter = 0;
        gameBoard.Clear();
        

        for(; counter < 64; counter++)
        {
             gameBoard.Add(new ChessGrid(new MultiPiece(), counter));
        }
        
        playerToMove = TeamColor.White;
        return true;
    }


    public void LoadPositionsFromFENString(string inputFEN)
    {
        gameBoard.Clear();
        var processed = inputFEN.Split(" ");
        int counter = 0;
        foreach (var character in processed[0])
        {
            MultiPiece tmpPiece = new MultiPiece(); 
            
            switch (character)
            {
                case 'P':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.type = ChessPieceTypes.PAWN;
                    break;
                case 'R':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.type = ChessPieceTypes.ROOK;
                    break;
                case 'N':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.type = ChessPieceTypes.KNIGHT;
                    break;
                
                case 'B':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.type = ChessPieceTypes.BISHOP;
                    break;
                case 'Q':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.type = ChessPieceTypes.QUEEN;
                    break;
                case 'K':
                    tmpPiece.Colour = TeamColor.White;
                    tmpPiece.type = ChessPieceTypes.KING;
                    break;
               
                case 'p':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.type = ChessPieceTypes.PAWN;
                    break;
                case 'r':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.type = ChessPieceTypes.ROOK;
                    break;
                case 'n':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.type = ChessPieceTypes.KNIGHT;
                    break;
                
                case 'b':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.type = ChessPieceTypes.BISHOP;
                    break;
                case 'q':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.type = ChessPieceTypes.QUEEN;
                    break;
                case 'k':
                    tmpPiece.Colour = TeamColor.Black;
                    tmpPiece.type = ChessPieceTypes.KING;
                    break;
                
                default:
                    break;
            }

            gameBoard[counter].pieceOnGrid = tmpPiece;
            counter++;
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
}
