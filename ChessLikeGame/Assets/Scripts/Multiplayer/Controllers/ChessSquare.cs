using System;
using System.Collections.Generic;
using Multiplayer.Models.BoardState;
using Multiplayer.View.DisplayData;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChessSquare : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color possibleMoveColor = Color.magenta;
    [SerializeField] private Color whiteColor = Color.white;
    [SerializeField] private Color blackColor = Color.blue;
    [SerializeField] private Dictionary<GridColor,Color> teamColors = new Dictionary<GridColor, Color>();
    private int id;
    private Renderer squareRenderer;
    private bool isSelected;
    private bool isPossibleMove;
    [SerializeField] public TMP_Text _text;
    private ChessGridInfoPanel info;

    public static event Action<int> onSelectedSquareEvent; 
    public static event Action<int> onPossibleMoveSelected; 

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        squareRenderer = GetComponent<Renderer>();
        teamColors.Add(GridColor.BLACK, blackColor);
        teamColors.Add(GridColor.WHITE, whiteColor);
        SetColor(normalColor);
        info = FindObjectOfType<ChessGridInfoPanel>();
    }

    private void Update()
    {
       // _text.GameObject().transform.LookAt(_camera.GameObject().transform);
    }

    public bool getIsSelected()
    {
        return isSelected;
    }

    public bool getIsPossibleMove()
    {
        return isPossibleMove;
    }
    
    public void SetText()
    {
        _text.text = ChessGrid.GetKeyFromIndex(id);
        gameObject.name = "CHESS_GRID:" + ChessGrid.GetKeyFromIndex(id);
    }

    public void SetNormalColor(GridColor color)
    {
        normalColor = teamColors[color];
        SetColor(normalColor);
    }

    private void SetColor(Color color)
    {
        squareRenderer.material.color = color;
        
    }

    public void OnMouseOver()
    {
        
        if (!isSelected && !isPossibleMove)
        {
            SetColor(hoverColor);
        }
    }

    public void OnMouseEnter()
    {
       info.UpdateInfoText(FindObjectOfType<MultiplayerDirector>().getChessEngine().GetGameBoardList()[id].PieceOnGrid.ToString());
        
    }

    public void OnMouseExit()
    {
        if (!isSelected && !isPossibleMove)
        {
            SetColor(normalColor);
        }
    }

   
    public void OnSelect(BaseEventData eventData)
    {
        if (isPossibleMove)
        {
            onPossibleMoveSelected?.Invoke(id);
            return;
        }
        isSelected = true;
        SetColor(selectedColor);
        onSelectedSquareEvent?.Invoke(id);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        
        isSelected = false;
        SetColor(isPossibleMove ? possibleMoveColor : normalColor);
    }

    public void SetID(int newID)
    {
        id = newID;
        SetText();
    }

    public int GetID()
    {
        return id;
    }

    public void SetPossibleMove(bool possibleMove)
    {
        if (isSelected && !isPossibleMove)
        {
            return;
        }
        isPossibleMove = possibleMove;
        SetColor(possibleMove ? possibleMoveColor : normalColor);
    }
}