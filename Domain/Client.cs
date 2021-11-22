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
        public static Client DecodeClient(string s)
        {
            string[] arr = s.Split(Logic.GameTransferSeparator);
            if (arr.Length == 3)
            {
                string username = arr[0];
                List<int> ownedGames = Logic.DecodeOwnedGames(arr[1]);
                bool isOnline = bool.Parse(arr[2]);
                return new Client(username)
                {
                    OwnedGames = ownedGames,
                    IsOnline = isOnline
                };
            }
            return null;
        }
        public string EncodeClient()
        {
            string ret = Username;
            ret += Logic.GameTransferSeparator + Logic.EncodeOwnedGames(OwnedGames);
            ret += Logic.GameTransferSeparator + IsOnline;
            return ret;
        }

        public override bool Equals(object obj)
        {
            Client c = (Client)obj;
            return Username.Equals(c.Username);
        }
    }
}
