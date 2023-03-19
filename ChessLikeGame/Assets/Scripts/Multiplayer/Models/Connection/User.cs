using System;

namespace Multiplayer.Models.Connection
{
    public class User
    {
        public User(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || this.GetType() == typeof(User))
            {
                return false;
            }
            User user = (User) obj;
            if (user.Username == this.Username)
            {
                return true;
            }
            return false;
        }
    }
}