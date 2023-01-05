using System;
using Chess.Control;
using Chess.Networking;
using Chess.Pieces;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Chess.UI
{
    public class ControlItemButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] internal Image IconActive;
        [SerializeField] internal Image IconInactive;
        [SerializeField] internal TMP_Text priceText;
        public event Action<ControlItemButton> OnButtonActive;
        public event Action<ControlItemButton> OnEnterButton;
        public event Action<ControlItemButton> OnExitButton;
        public event Action<ControlItemButton> OnButtonDeactivate;
        public event Action<ControlItemButton> OnButtonDestroy;
        internal ChessPiece ThisControlItem;
        internal NetworkPlayerChess PlayerChess;

        public ChessPiece StationOrUnit { get => ThisControlItem; set => ThisControlItem = value; }

        internal void Activate()
        {
            OnButtonActive?.Invoke(this);
        }

        internal void Deactivate()
        {
            OnButtonDeactivate?.Invoke(this);
        }

        private void OnDestroy()
        {
            OnButtonDestroy?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnterButton?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExitButton?.Invoke(this);
        }
    }
}