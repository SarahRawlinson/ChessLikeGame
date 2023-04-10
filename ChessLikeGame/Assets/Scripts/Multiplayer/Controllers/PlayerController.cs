using System;
using System.Collections.Generic;
using LibObjects;
using Multiplayer.Models;
using UnityEngine;

namespace Multiplayer.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        private User host;
        private void Start()
        {
            List<string> data = new List<string>() {"test", "test"};
            Message message = new Message(Message.GameCommand.AskToMakeMove, data);
        }
    }
}