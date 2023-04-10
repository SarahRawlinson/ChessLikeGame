using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Multiplayer.Models
{
    [Serializable]
    public class Message
    {
        public enum GameCommand
        {
            AskToMakeMove,
            YourMove
        }

        public GameCommand command;
        public List<string> data;

        public Message(GameCommand command, List<string> data)
        {
            this.command = command;
            this.data = data;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Message GetMessageFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Message>(json);
        }
    }
}