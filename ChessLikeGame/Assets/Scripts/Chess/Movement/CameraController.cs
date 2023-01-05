using System;
using Chess.Control;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chess.Movement
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private Transform playerCamera = null;
        [SerializeField] private float speed = 20f;
        [SerializeField] private float screenBoarderThickness = 10f;
        [SerializeField] private Vector2 screenXLimits = Vector2.zero;
        [SerializeField] private Vector2 screenZLimits = Vector2.zero;
        [SerializeField] private Vector2 screenYLimits = Vector2.zero;
        [SerializeField] private float scrollSpeed = 10f;
        [SerializeField] private Transform startingPosition;
        [SerializeField] private GameObject marker = null;
        [SyncVar]
        private bool startMovement = false;
        private float mouseScrollY;

        private Controls controls;
        private Vector2 previousImput;
        private Camera cam;

        public Transform StartingPosition { get => startingPosition; }

        public override void OnStartAuthority()
        {
            cam = Camera.main;
            playerCamera.gameObject.SetActive(true);
            controls = new Controls();
            controls.Player.MoveCamera.performed += SetPreviousInput;
            controls.Player.MoveCamera.canceled += SetPreviousInput;
            controls.Player.CameraZoom.performed += x => mouseScrollY = x.ReadValue<float>();
            controls.Enable();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!hasAuthority) return;;
            if (marker != null) marker.SetActive(true);
        }

        [Server]
        public void SceneLoaded()
        {
            moveCameraTo(startingPosition.position);
            startMovement = true;
        }

        public void moveCameraTo(Vector3 position)
        {
            transform.position = position;
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority || !Application.isFocused) return;
            if (!startMovement) return;
            UpdateCameraPosition();
        }


        private float MouseScrollZoom()
        {
            float y = 0;
            if (mouseScrollY > 0)
            {
                y -= scrollSpeed;
            }
            if (mouseScrollY < 0)
            {
                y += scrollSpeed;
            }
            return y;
        }

        private void UpdateCameraPosition()
        {
            Vector3 pos = playerCamera.position;
            float originalY = pos.y;
            Vector3 markerPos = marker.transform.position;
            Vector3 cursorMovement = Vector3.zero;
            if (previousImput == Vector2.zero)
            {
                Vector2 cursorPosition = Mouse.current.position.ReadValue();
                if (cursorPosition.y >= Screen.height - screenBoarderThickness)
                {
                    cursorMovement.z += 1;
                }
                else if (cursorPosition.y <= screenBoarderThickness)
                {
                    cursorMovement.z -= 1;
                }
                if (cursorPosition.x >= Screen.width - screenBoarderThickness)
                {
                    cursorMovement.x += 1;
                }
                else if (cursorPosition.x <= screenBoarderThickness)
                {
                    cursorMovement.x -= 1;
                }

                pos += cursorMovement.normalized * speed * Time.deltaTime;
            }
            else
            {
                pos += new Vector3(previousImput.x, 0f, previousImput.y) * speed * Time.deltaTime;
                markerPos += new Vector3(previousImput.x, 0f, previousImput.y) * speed * Time.deltaTime;;
            }
            float Zoom = MouseScrollZoom();
            float y = pos.y;
            pos.y = Mathf.Clamp(pos.y + Zoom, screenYLimits.x, screenYLimits.y);
            if (Math.Abs(y - pos.y) < 0.05f)
            {
                pos.z = Mathf.Clamp(pos.z, screenXLimits.x, screenXLimits.y);
            }
            else
            {
                
                pos.z = Mathf.Clamp(pos.z - Zoom, screenXLimits.x, screenXLimits.y);
            }            
            pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y); 
            
            playerCamera.position = pos;
            if (marker != null)
            {
                var position = playerCamera.position;
                float size = position.y / 2;
                marker.transform.localScale = new Vector3(size, size, size);
                marker.transform.position = 
                    new Vector3(position.x, marker.transform.position.y, position.z + (size * 2));
            }
        }

        private void SetPreviousInput(InputAction.CallbackContext ctx)
        {            
            previousImput = ctx.ReadValue<Vector2>();
        }
    }
}