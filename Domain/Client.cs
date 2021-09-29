using System.Collections.Generic;

namespace Domain
{
    public class Client
    {
        public string Username { get; private set; }
        public List<int> OwnedGames { get; set; }
        public bool IsOnline { get; set; }

        public Client(string Username)
        {
            this.Username = Username;
            OwnedGames = new List<int>();
            IsOnline = false;
        }
        public override bool Equals(object obj)
        {
            Client c = (Client)obj;
            return Username.Equals(c.Username);
        }
    }
}
