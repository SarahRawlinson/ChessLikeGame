﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer.Controllers
{
    public class GameObjectController: MonoBehaviour
    {
        [SerializeField] private GameObject whitePawnPrefab;
        [SerializeField] private GameObject whiteRookPrefab;
        [SerializeField] private GameObject whiteKnightPrefab;
        [SerializeField] private GameObject whiteBishopPrefab;
        [SerializeField] private GameObject whiteQueenPrefab;
        [SerializeField] private GameObject whiteKingPrefab;
        [SerializeField] private GameObject blackPawnPrefab;
        [SerializeField] private GameObject blackRookPrefab;
        [SerializeField] private GameObject blackKnightPrefab;
        [SerializeField] private GameObject blackBishopPrefab;
        [SerializeField] private GameObject blackQueenPrefab;
        [SerializeField] private GameObject blackKingPrefab;
        private List<GameObject> _objectsInUse = new();


        private int SpawnObject(GameObject gameObject, Vector3 position)
        {
            GameObject newGameObject = Instantiate(gameObject, position, Quaternion.identity);
            _objectsInUse.Add(newGameObject);
            return _objectsInUse.Count-1;
        }
        
        public int SpawnPiece(ChessPieceTypes type, TeamColor teamColor, Vector3 position)
        {
            switch (teamColor)
            {
                case TeamColor.Black:
                    switch (type)
                    {
                        case ChessPieceTypes.NONE:
                            break;
                        case ChessPieceTypes.PAWN:
                            return SpawnObject(blackPawnPrefab, position );
                        case ChessPieceTypes.BISHOP:
                            return SpawnObject(blackBishopPrefab, position );
                        case ChessPieceTypes.KNIGHT:
                            return SpawnObject(blackKnightPrefab, position );
                        case ChessPieceTypes.ROOK:
                            return SpawnObject(blackRookPrefab, position );
                        case ChessPieceTypes.QUEEN:
                            return SpawnObject(blackQueenPrefab, position );
                        case ChessPieceTypes.KING:
                            return SpawnObject(blackKingPrefab, position );
                        default:
                            throw new ArgumentOutOfRangeException(nameof(teamColor), teamColor, null);
                    }

                    break;
                
                case TeamColor.White:
                    switch (type)
                    {
                        case ChessPieceTypes.NONE:
                            break;
                        case ChessPieceTypes.PAWN:
                            return SpawnObject(whitePawnPrefab, position );
                        case ChessPieceTypes.BISHOP:
                            return SpawnObject(whiteBishopPrefab, position );
                        case ChessPieceTypes.KNIGHT:
                            return SpawnObject(whiteKnightPrefab, position );
                        case ChessPieceTypes.ROOK:
                            return SpawnObject(whiteRookPrefab, position );
                        case ChessPieceTypes.QUEEN:
                            return SpawnObject(whiteQueenPrefab, position );
                        case ChessPieceTypes.KING:
                            return SpawnObject(whiteKingPrefab, position );
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamColor), teamColor, null);
            }

            return -1;
        }
        
    }

}