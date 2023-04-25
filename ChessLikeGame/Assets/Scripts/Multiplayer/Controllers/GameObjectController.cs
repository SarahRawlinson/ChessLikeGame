using System;
using System.Collections.Generic;
using System.Linq;
using Multiplayer.Models.BoardState;
using UnityEngine;
using UnityEngine.EventSystems;

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
        [SerializeField] private GameObject StartGameSquarePosition;
        private List<GameObject> _objectsInUse = new List<GameObject>();
        private List<int> gameSquares = new List<int>();

        [SerializeField] private ChessSquare positionGameObjectPrefab; // Size of a Sqaure on the board.
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private Camera mainCamera;
   

        public  void CreateBoardPositions(int x, int y)
        {
            float size = positionGameObjectPrefab.transform.localScale.x;
            
            for (int a = 0; a < x; a++)
            {
                for (int b = 0; b < y; b++)
                {
                    gameSquares.Add(SpawnObject(positionGameObjectPrefab.gameObject,  new Vector3(size * a + StartGameSquarePosition.transform.position.x,StartGameSquarePosition.transform.position.y, -size * b + StartGameSquarePosition.transform.position.z)));
                    _objectsInUse.Last().GetComponent<ChessSquare>().SetID(gameSquares.Last());
                    GridColor squarecolor = ChessGrid.CalculateGridColorFromLocation(gameSquares.Last());
                    _objectsInUse.Last().GetComponent<ChessSquare>().SetNormalColor(squarecolor);
                }
                
            }
        }

        public GameObject GetGameObjectFromArray(int index)
        {
            return _objectsInUse[index];
        }

        public Vector3 GetPositionVectorfromGameSquare(int index )
        {
            return GetGameObjectFromArray(gameSquares[index]).transform.position;
        }

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
        
private void Update()
{
    // Check for cursor key inputs
    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
    {
        // Get the current selected game object
        GameObject currentSelected = eventSystem.currentSelectedGameObject;
        int currentIndex = currentSelected != null ? currentSelected.GetComponent<ChessSquare>().GetID() : -1;

        // Get camera's forward and right vectors projected on the XZ plane
        Vector3 cameraForwardXZ = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(mainCamera.transform.right.x, 0, mainCamera.transform.right.z).normalized;

        // Calculate the next game object index based on the camera-relative input
        int nextIndex = currentIndex;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Vector3.Dot(cameraForwardXZ, Vector3.forward) > 0.5f && currentIndex >= 8) nextIndex -= 8;
            if (Vector3.Dot(cameraForwardXZ, Vector3.back) > 0.5f && currentIndex < 56) nextIndex += 8;
            if (Vector3.Dot(cameraForwardXZ, Vector3.right) > 0.5f && currentIndex % 8 < 7) nextIndex += 1;
            if (Vector3.Dot(cameraForwardXZ, Vector3.left) > 0.5f && currentIndex % 8 > 0) nextIndex -= 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Vector3.Dot(cameraForwardXZ, Vector3.forward) > 0.5f && currentIndex < 56) nextIndex += 8;
            if (Vector3.Dot(cameraForwardXZ, Vector3.back) > 0.5f && currentIndex >= 8) nextIndex -= 8;
            if (Vector3.Dot(cameraForwardXZ, Vector3.right) > 0.5f && currentIndex % 8 > 0) nextIndex -= 1;
            if (Vector3.Dot(cameraForwardXZ, Vector3.left) > 0.5f && currentIndex % 8 < 7) nextIndex += 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Vector3.Dot(cameraRightXZ, Vector3.forward) > 0.5f && currentIndex >= 8) nextIndex -= 8;
            if (Vector3.Dot(cameraRightXZ, Vector3.back) > 0.5f && currentIndex < 56) nextIndex += 8;
            if (Vector3.Dot(cameraRightXZ, Vector3.right) > 0.5f && currentIndex % 8 < 7) nextIndex += 1;
            if (Vector3.Dot(cameraRightXZ, Vector3.left) > 0.5f && currentIndex % 8 > 0) nextIndex -= 1;
        }
        
        {
            if (Vector3.Dot(cameraRightXZ, Vector3.forward) > 0.5f && currentIndex < 56) nextIndex += 8;
            if (Vector3.Dot(cameraRightXZ, Vector3.back) > 0.5f && currentIndex >= 8) nextIndex -= 8;
            if (Vector3.Dot(cameraRightXZ, Vector3.right) > 0.5f && currentIndex % 8 > 0) nextIndex -= 1;
            if (Vector3.Dot(cameraRightXZ, Vector3.left) > 0.5f && currentIndex % 8 < 7) nextIndex += 1;
        }

// Make sure nextIndex is within the range of gameSquares
        nextIndex = Mathf.Clamp(nextIndex, 0, gameSquares.Count - 1);

// Set the new selected game object
        GameObject nextSelected = GetGameObjectFromArray(gameSquares[nextIndex]);
        eventSystem.SetSelectedGameObject(nextSelected);
    }

    // Check for mouse input
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            ChessSquare square = hit.collider.GetComponent<ChessSquare>();
            if (square != null)
            {
                if (eventSystem.currentSelectedGameObject != null)
                {
                    ChessSquare previouslySelectedSquare = eventSystem.currentSelectedGameObject.GetComponent<ChessSquare>();
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

    if (Input.GetKeyDown(KeyCode.Tab))
    {
        SelectNextSquare();
    }
}



private void SelectNextSquare()
{
    if (eventSystem.currentSelectedGameObject == null) return;

    GameObject currentSelected = eventSystem.currentSelectedGameObject;
    int currentIndex = currentSelected.GetComponent<ChessSquare>().GetID();

    int nextIndex = (currentIndex + 1) % gameSquares.Count;
    GameObject nextSelected = GetGameObjectFromArray(gameSquares[nextIndex]);
    eventSystem.SetSelectedGameObject(nextSelected);
}





    

        public void MoveGameObject(int gameObjectIndex, Vector3 moveVector)
        {
            _objectsInUse[gameObjectIndex].transform.position = moveVector;
        }

        public void RemoveGameObjectFromGame(int gameObjectIndex)
        {
            _objectsInUse[gameObjectIndex].SetActive(false);
        }
    }

}