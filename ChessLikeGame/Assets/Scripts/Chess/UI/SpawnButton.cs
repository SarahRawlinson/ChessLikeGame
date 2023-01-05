using System;
using Chess.Control;
using Chess.Networking;
using Chess.Pieces;
using Chess.Resources;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Chess.UI
{
    public class SpawnButton : ControlItemButton, IPointerDownHandler, IPointerUpHandler
    {
        ControlItem stationOrUnit;
        RTSUnitSpawner spawner;
        private int spawnerUnitIndex = -1;
        private bool spawn = false;
        private Camera mainCamera;


        public void Start()
        {
            try
            {
                PlayerChess = NetworkClient.connection.identity.GetComponent<NetworkPlayerChess>();
            }
            catch (Exception e)
            {
                try
                {
                    Debug.Log($"{NetworkClient.connection.identity.gameObject.name} {e.Message}");
                }
                catch
                {
                    Debug.Log($"{e.Message}");
                }

                return;
            }
            mainCamera = Camera.main;
        }

        public void SetSpawner(RTSUnitSpawner spawn, int unitID, ControlItem item)
        {
            ThisControlItem = (ChessPiece) item;
            spawner = spawn;
            spawnerUnitIndex = unitID;
            stationOrUnit = item;
            priceText.text = item.price.ToString();
            IconActive.sprite = item.Icon;
            IconInactive.sprite = item.Icon;
        }

        private void Update()
        {
            ResourceTracker resourceTracker = NetworkClient.connection.identity.GetComponent<ResourceTracker>();
            //if (Mouse.current.leftButton.wasPressedThisFrame && Keyboard.current.ctrlKey.isPressed) SetUpBuild();
            if (resourceTracker.Resources < stationOrUnit.price && IconActive.enabled)
            {
                //Debug.Log($"Not Enough Resources for {stationOrUnit.gameObject.name}");
                IconActive.enabled = false;
            }
            if (resourceTracker.Resources > stationOrUnit.price && !IconActive.enabled)
            {
                //Debug.Log($"Now Have Enough Resources for {stationOrUnit.gameObject.name}");
                IconActive.enabled = true;
            }
        }

        public void SpawnItem()
        {
            //Debug.Log("Spawn");
            spawner.SpawnObject(spawnerUnitIndex);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //OnButtonActive?.Invoke(this);
            Activate();
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!spawn) SpawnItem();
            spawn = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //OnButtonDeactivate?.Invoke(this);
            Deactivate();
            spawn = false;
        }

        //private void OnDestroy()
        //{
        //    OnButtonDestroy?.Invoke(this);
        //}
    }
}