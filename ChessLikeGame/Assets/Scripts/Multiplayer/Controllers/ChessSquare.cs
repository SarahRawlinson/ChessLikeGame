using System;
using System.Collections.Generic;
using Multiplayer.Models.BoardState;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChessSquare : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color possibleMoveColor = Color.magenta;
    [SerializeField] private Dictionary<GridColor,Color> teamColors = new Dictionary<GridColor, Color>();
    private int id;
    private Renderer squareRenderer;
    private bool isSelected;
    private bool isPossibleMove;

    private void Awake()
    {
        squareRenderer = GetComponent<Renderer>();
        teamColors.Add(GridColor.BLACK, Color.blue);
        teamColors.Add(GridColor.WHITE, Color.grey);
        SetColor(normalColor);
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

    public void OnMouseExit()
    {
        if (!isSelected && !isPossibleMove)
        {
            SetColor(normalColor);
        }
    }

    public void Select()
    {
        isSelected = true;
        SetColor(selectedColor);
    }
    
    public void Deselect()
    {
        isSelected = false;
        SetColor(normalColor);
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        SetColor(selectedColor);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        SetColor(isPossibleMove ? possibleMoveColor : normalColor);
    }

    public void SetID(int newID)
    {
        id = newID;
    }

    public int GetID()
    {
        return id;
    }

    public void SetPossibleMove(bool possibleMove)
    {
        isPossibleMove = possibleMove;
        SetColor(possibleMove ? possibleMoveColor : normalColor);
    }
}