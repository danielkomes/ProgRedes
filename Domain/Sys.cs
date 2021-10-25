using System.Collections.Generic;

namespace Domain
{
    public static class Sys
    {
        public static int IdCounter { get; set; }
        public static List<Game> Games { get; set; }
        public static List<Client> Clients { get; set; }
        private static object locker;

        static Sys()
        {
            locker = new object();
            Games = new List<Game>();
            Clients = new List<Client>();
        }
        public static Client GetClient(string username)
        {
            lock (locker)
            {
                return Clients.Find(c => c.Username.Equals(username));
            }
        }
        public static bool AddClient(string username)
        {
            lock (locker)
            {
                bool ret = false;
                Client c = new Client(username);
                if (!Clients.Contains(c))
                {
                    Clients.Add(c);
                    ret = true;
                }
                return ret;
            }
        }
        public static void DeleteClient(string username)
        {
            lock (locker)
            {
                Client c = GetClient(username);
                Clients.Remove(c);
            }
        }
        public static int GetNewId()
        {
            lock (locker)
            {
                return IdCounter++;
            }
        }

        public static void AddGame(Game game)
        {
            lock (locker)
            {
                game.Id = GetNewId();
                Games.Add(game);
            }
        }
        public static void DeleteGame(Game game)
        {
            lock (locker)
            {
                Games.Remove(game);
                foreach (Client c in Clients)
                {
                    c.OwnedGames.Remove(game.Id);
                }
            }
        }
        public static bool BuyGame(string username, int id)
        {
            lock (locker)
            {
                bool ret = false;
                Client client = GetClient(username);
                if (Games.Find(g => g.Id == id) != null)
                {
                    if (!client.OwnedGames.Contains(id))
                    {
                        client.OwnedGames.Add(id);
                        ret = true;
                    }
                }
                return ret;
            }
        }
        public static bool RemoveGameFromClient(string username, int id)
        {
            lock (locker)
            {
                bool ret = false;
                Client client = GetClient(username);
                ret = client.OwnedGames.Remove(id);
                return ret;
            }
        }
        public static void RemoveAllGamesFromClient(string username)
        {
            lock (locker)
            {
                Client c = GetClient(username);
                c.OwnedGames.Clear();
            }
        }
        public static void ReplaceGame(Game game)
        {
            lock (locker)
            {
                for (int i = 0; i < Games.Count; i++)
                {
                    if (Games[i].Equals(game))
                    {
                        Games[i] = game;
                        return;
                    }
                }
            }
        }
        public static void AddReview(int id, Review review)
        {
            lock (locker)
            {
                Game g = Games.Find(g => g.Id == id);
                if (g != null)
                {
                    g.Reviews.Add(review);
                }
            }

        }
    }
}
