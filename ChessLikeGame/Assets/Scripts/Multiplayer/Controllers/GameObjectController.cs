using System;
using System.Collections.Generic;
using System.Linq;
using Multiplayer.Models.BoardState;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Multiplayer.Controllers
{
    public class GameObjectController : MonoBehaviour
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
        [SerializeField] private GameObject StartGameSquarePosition;
        private readonly List<GameObject> _objectsInUse = new();
        private readonly List<int> gameSquares = new();

        [SerializeField] private ChessSquare positionGameObjectPrefab; // Size of a Sqaure on the board.
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float cameraAngleOffset;
        [SerializeField] private bool invertXAxis;
        [SerializeField] private bool invertYAxis;
        [SerializeField] private float angleTolerance = 30f;
        [SerializeField] private float axisFlipOffset;
        [SerializeField] private float yOffset = 1;


        public ChessSquare GetChessCube(int index)
        {
           return  _objectsInUse[gameSquares[index]].GetComponent<ChessSquare>();
        }
        
        public void CreateBoardPositions(int x, int y)
        {
            var size = positionGameObjectPrefab.transform.localScale.x;

            for (var a = 0; a < x; a++)
            for (var b = 0; b < y; b++)
            {
                gameSquares.Add(SpawnObject(positionGameObjectPrefab.gameObject,
                    new Vector3(size * a + StartGameSquarePosition.transform.position.x,
                        StartGameSquarePosition.transform.position.y,
                        -size * b + StartGameSquarePosition.transform.position.z)));
                _objectsInUse.Last().GetComponent<ChessSquare>().SetID(gameSquares.Last());
                var squarecolor = ChessGrid.CalculateGridColorFromLocation(gameSquares.Last());
                _objectsInUse.Last().GetComponent<ChessSquare>().SetNormalColor(squarecolor);
            }
        }

        public GameObject GetGameObjectFromArray(int index)
        {
            return _objectsInUse[index];
        }

        public Vector3 GetPositionVectorfromGameSquare(int index)
        {
            return GetGameObjectFromArray(gameSquares[index]).transform.position;
        }

        private int SpawnObject(GameObject gameObject, Vector3 position)
        {
            var newGameObject = Instantiate(gameObject, position, Quaternion.identity);
            _objectsInUse.Add(newGameObject);
            return _objectsInUse.Count - 1;
        }

        public int SpawnPiece(ChessPieceTypes type, TeamColor teamColor, Vector3 position)
        {
            position.y += yOffset;
            switch (teamColor)
            {
                case TeamColor.Black:
                    switch (type)
                    {
                        case ChessPieceTypes.NONE:
                            break;
                        case ChessPieceTypes.PAWN:
                            return SpawnObject(blackPawnPrefab, position);
                        case ChessPieceTypes.BISHOP:
                            return SpawnObject(blackBishopPrefab, position);
                        case ChessPieceTypes.KNIGHT:
                            return SpawnObject(blackKnightPrefab, position);
                        case ChessPieceTypes.ROOK:
                            return SpawnObject(blackRookPrefab, position);
                        case ChessPieceTypes.QUEEN:
                            return SpawnObject(blackQueenPrefab, position);
                        case ChessPieceTypes.KING:
                            return SpawnObject(blackKingPrefab, position);
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
                            return SpawnObject(whitePawnPrefab, position);
                        case ChessPieceTypes.BISHOP:
                            return SpawnObject(whiteBishopPrefab, position);
                        case ChessPieceTypes.KNIGHT:
                            return SpawnObject(whiteKnightPrefab, position);
                        case ChessPieceTypes.ROOK:
                            return SpawnObject(whiteRookPrefab, position);
                        case ChessPieceTypes.QUEEN:
                            return SpawnObject(whiteQueenPrefab, position);
                        case ChessPieceTypes.KING:
                            return SpawnObject(whiteKingPrefab, position);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamColor), teamColor, null);
            }

            return -1;
        }

        private void Update()
        {
            // Check for cursor key inputs
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                // Get the current selected game object
                var currentSelected = eventSystem.currentSelectedGameObject;
                var currentIndex = currentSelected != null ? currentSelected.GetComponent<ChessSquare>().GetID() : -1;

                // Get camera's forward and right vectors projected on the XZ plane
                var cameraForwardXZ = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z)
                    .normalized;
                var cameraRightXZ = new Vector3(mainCamera.transform.right.x, 0, mainCamera.transform.right.z)
                    .normalized;

                // Apply the camera angle offset
                var rotation = Quaternion.Euler(0, cameraAngleOffset, 0);
                cameraForwardXZ = rotation * cameraForwardXZ;
                cameraRightXZ = rotation * cameraRightXZ;

                // Determine the current camera quadrant
                var cameraQuadrant = 0;

                var isFacingBlackSide = mainCamera.transform.eulerAngles.y > 90 + axisFlipOffset &&
                                        mainCamera.transform.eulerAngles.y < 270 + axisFlipOffset;


                if (cameraForwardXZ.z >= 0)
                    cameraQuadrant = cameraRightXZ.x >= 0 ? 1 : 4;
                else
                    cameraQuadrant = cameraRightXZ.x >= 0 ? 2 : 3;

                // Remap arrow keys based on the camera quadrant
                var upDirection = Vector3.zero;
                var downDirection = Vector3.zero;
                var leftDirection = Vector3.zero;
                var rightDirection = Vector3.zero;

                switch (cameraQuadrant)
                {
                    case 1:
                        upDirection = isFacingBlackSide ? -cameraForwardXZ : cameraForwardXZ;
                        downDirection = isFacingBlackSide ? cameraForwardXZ : -cameraForwardXZ;
                        leftDirection = isFacingBlackSide ? cameraRightXZ : -cameraRightXZ;
                        rightDirection = isFacingBlackSide ? -cameraRightXZ : cameraRightXZ;
                        break;
                    case 2:
                        upDirection = isFacingBlackSide ? -cameraRightXZ : cameraRightXZ;
                        downDirection = isFacingBlackSide ? cameraRightXZ : -cameraRightXZ;
                        leftDirection = isFacingBlackSide ? -cameraForwardXZ : cameraForwardXZ;
                        rightDirection = isFacingBlackSide ? cameraForwardXZ : -cameraForwardXZ;
                        break;
                    case 3:
                        upDirection = isFacingBlackSide ? cameraForwardXZ : -cameraForwardXZ;
                        downDirection = isFacingBlackSide ? -cameraForwardXZ : cameraForwardXZ;
                        leftDirection = isFacingBlackSide ? -cameraRightXZ : cameraRightXZ;
                        rightDirection = isFacingBlackSide ? cameraRightXZ : -cameraRightXZ;
                        break;
                    case 4:
                        upDirection = isFacingBlackSide ? cameraRightXZ : -cameraRightXZ;
                        downDirection = isFacingBlackSide ? -cameraRightXZ : cameraRightXZ;
                        leftDirection = isFacingBlackSide ? cameraForwardXZ : -cameraForwardXZ;
                        rightDirection = isFacingBlackSide ? -cameraForwardXZ : cameraForwardXZ;
                        break;
                }

                // Calculate the next game object index based on the camera-relative input
                var nextIndex = currentIndex;
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    nextIndex = GetNextIndexBasedOnCameraDirection(currentIndex,
                        invertYAxis ? downDirection : upDirection);
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    nextIndex = GetNextIndexBasedOnCameraDirection(currentIndex,
                        invertYAxis ? upDirection : downDirection);
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    nextIndex = GetNextIndexBasedOnCameraDirection(currentIndex,
                        invertXAxis ? rightDirection : leftDirection);
                if (Input.GetKeyDown(KeyCode.RightArrow))
                    nextIndex = GetNextIndexBasedOnCameraDirection(currentIndex,
                        invertXAxis ? leftDirection : rightDirection);

                // Make sure nextIndex is within the range of gameSquares

                nextIndex = Mathf.Clamp(nextIndex, 0, gameSquares.Count - 1);

                // Set the new selected game object
                var nextSelected = GetGameObjectFromArray(gameSquares[nextIndex]);
                eventSystem.SetSelectedGameObject(nextSelected);
            }

            // Check for mouse input
            if (Input.GetMouseButtonDown(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    var square = hit.collider.GetComponent<ChessSquare>();
                    if (square != null)
                    {
                        if (eventSystem.currentSelectedGameObject != null)
                        {
                            var previouslySelectedSquare =
                                eventSystem.currentSelectedGameObject.GetComponent<ChessSquare>();
                            if (previouslySelectedSquare != null)
                            {
                                // previouslySelectedSquare.Deselect();
                            }
                        }

                        // square.Select();
                        eventSystem.SetSelectedGameObject(square.gameObject);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab)) SelectNextSquare();
        }
        
        private int GetNextIndexBasedOnCameraDirection(int currentIndex, Vector3 direction)
        {
            var nextIndex = currentIndex;

            Vector3[] cardinalDirections = {Vector3.forward, Vector3.back, Vector3.right, Vector3.left};
            var nearestDirection = Vector3.zero;
            var minAngle = float.MaxValue;

            foreach (var cardinalDirection in cardinalDirections)
            {
                var angle = Vector3.Angle(direction, cardinalDirection);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    nearestDirection = cardinalDirection;
                }
            }

            if (minAngle > angleTolerance) return currentIndex;

            if (nearestDirection == Vector3.forward && currentIndex >= 8) nextIndex -= 8;
            if (nearestDirection == Vector3.back && currentIndex < 56) nextIndex += 8;
            if (nearestDirection == Vector3.right && currentIndex % 8 < 7) nextIndex += 1;
            if (nearestDirection == Vector3.left && currentIndex % 8 > 0) nextIndex -= 1;

            return nextIndex;
        }

        private void SelectNextSquare()
        {
            if (eventSystem.currentSelectedGameObject == null) return;

            var currentSelected = eventSystem.currentSelectedGameObject;
            var currentIndex = currentSelected.GetComponent<ChessSquare>().GetID();

            var nextIndex = (currentIndex + 1) % gameSquares.Count;
            var nextSelected = GetGameObjectFromArray(gameSquares[nextIndex]);
            eventSystem.SetSelectedGameObject(nextSelected);
        }

        public void MoveGameObject(int gameObjectIndex, Vector3 moveVector)
        {
            _objectsInUse[gameObjectIndex].transform.position = new Vector3( moveVector.x, moveVector.y + yOffset, moveVector.z) ;
        }

        public void RemoveGameObjectFromGame(int gameObjectIndex)
        {
            _objectsInUse[gameObjectIndex].SetActive(false);
        }
    }
}