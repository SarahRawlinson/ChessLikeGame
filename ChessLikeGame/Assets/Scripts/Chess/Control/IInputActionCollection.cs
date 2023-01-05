﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Chess.Control
{
    public interface IInputActionCollection : IEnumerable<InputAction>, IEnumerable
    {
        InputBinding? bindingMask { get; set; }

        ReadOnlyArray<InputDevice>? devices { get; set; }

        ReadOnlyArray<InputControlScheme> controlSchemes { get; }

        bool Contains(InputAction action);

        void Enable();

        void Disable();
    }
}