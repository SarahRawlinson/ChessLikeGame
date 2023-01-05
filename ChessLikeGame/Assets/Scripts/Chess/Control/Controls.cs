
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Chess.Control
{
    public class Controls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""RTSControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""e1af7549-0fe3-42e2-9862-22f009a9f6be"",
            ""actions"": [
                {
                    ""name"": ""Move Camera"",
                    ""type"": ""Value"",
                    ""id"": ""26fc8f1f-b13b-48aa-854e-ffd40fa1e15a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera Zoom"",
                    ""type"": ""PassThrough"",
                    ""id"": ""2dbab0d2-965c-431d-9bd8-01866bc16aff"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""8e120610-575d-42ae-a65e-23134a482000"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Camera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""48ba3cc9-0df8-4b78-9d6d-9badba49f7f5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyBoard & Mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""67223208-2450-49ba-90d4-5d164c068982"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyBoard & Mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""4581807a-2f37-4977-b08a-45f557c198d0"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyBoard & Mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""39829470-87d1-4506-b694-0d162244f157"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyBoard & Mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""edef032a-fd31-49e0-866f-3ecb2d61fd41"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Camera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""dfeb5abc-b21d-4c34-9aeb-4d8de58e60cd"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyBoard & Mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""564e2233-906e-41c8-b44c-8c9c318c8849"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyBoard & Mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b58517ac-c140-48ea-b9b5-e1199275baf5"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyBoard & Mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""75873163-847f-4682-ad64-06bd52a0e2f0"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyBoard & Mouse"",
                    ""action"": ""Move Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""31122307-8c70-4b8d-bbea-36bea3cddad3"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyBoard & Mouse"",
                    ""action"": ""Camera Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyBoard & Mouse"",
            ""bindingGroup"": ""KeyBoard & Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_MoveCamera = m_Player.FindAction("Move Camera", throwIfNotFound: true);
            m_Player_CameraZoom = m_Player.FindAction("Camera Zoom", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_MoveCamera;
        private readonly InputAction m_Player_CameraZoom;
        public struct PlayerActions
        {
            private Controls m_Wrapper;
            public PlayerActions(Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @MoveCamera => m_Wrapper.m_Player_MoveCamera;
            public InputAction @CameraZoom => m_Wrapper.m_Player_CameraZoom;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    @MoveCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                    @MoveCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                    @MoveCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMoveCamera;
                    @CameraZoom.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraZoom;
                    @CameraZoom.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraZoom;
                    @CameraZoom.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraZoom;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @MoveCamera.started += instance.OnMoveCamera;
                    @MoveCamera.performed += instance.OnMoveCamera;
                    @MoveCamera.canceled += instance.OnMoveCamera;
                    @CameraZoom.started += instance.OnCameraZoom;
                    @CameraZoom.performed += instance.OnCameraZoom;
                    @CameraZoom.canceled += instance.OnCameraZoom;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);
        private int m_KeyBoardMouseSchemeIndex = -1;
        public InputControlScheme KeyBoardMouseScheme
        {
            get
            {
                if (m_KeyBoardMouseSchemeIndex == -1) m_KeyBoardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyBoard & Mouse");
                return asset.controlSchemes[m_KeyBoardMouseSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnMoveCamera(InputAction.CallbackContext context);
            void OnCameraZoom(InputAction.CallbackContext context);
        }
    }
}