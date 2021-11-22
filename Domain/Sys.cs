using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Domain
{
    public static class Sys
    {
        public static int IdCounter { get; set; }
        public static List<Game> Games { get; set; }
        public static List<Client> Clients { get; set; }
        private static object gamesLocker;
        private static object clientsLocker;

        static Sys()
        {
            gamesLocker = new object();
            clientsLocker = new object();
            Games = new List<Game>();
            Clients = new List<Client>();
        }
        public static List<Game> GetGames()
        {
            lock (gamesLocker)
            {
                return Games;
            }
        }
        public static List<Game> GetGames(string title, string genre, int rating, int page, int pageSize)
        {
            lock (gamesLocker)
            {
                List<Game> ret = new List<Game>();
                if (pageSize > 0 && page > 0)
                {
                    int count = Games.Count;
                    int start = page * pageSize - pageSize;
                    int end = Math.Min(start + pageSize, count);
                    if (start >= 0 && start < count)
                    {
                        for (int i = start; i < end; i++)
                        {
                            Game game = Games[i];
                            bool add = true;
                            if (add && !string.IsNullOrEmpty(title))
                            {
                                Match match = Regex.Match(game.Title, title);
                                if (match.Success)
                                {
                                    add &= true;
                                }
                                else
                                {
                                    add &= false;
                                }
                            }
                            if (add && !string.IsNullOrEmpty(genre))
                            {
                                bool isGenre = Enum.TryParse<EGenre>(genre, out EGenre g);
                                if (isGenre)
                                {
                                    if (game.Genre == g)
                                    {
                                        add &= true;
                                    }
                                    else
                                    {
                                        add &= false;
                                    }
                                }
                            }
                            if (add && rating >= 0)
                            {
                                if (game.AverageRating() == rating)
                                {
                                    add &= true;
                                }
                                else
                                {
                                    add &= false;
                                }
                            }
                            if (add)
                            {
                                ret.Add(game);
                            }
                        }
                    }
                }
                return ret;
            }
        }
        public static List<Review> GetReviews(int id, int page, int pageSize)
        {
            lock (gamesLocker)
            {
                List<Review> ret = new List<Review>();
                if (pageSize > 0 && page > 0)
                {
                    Game game = GetGame(id);
                    if (game != null)
                    {
                        List<Review> list = game.Reviews;
                        int count = list.Count;
                        int start = page * pageSize - pageSize;
                        int end = Math.Min(start + pageSize, list.Count);
                        if (start >= 0 && start < list.Count)
                        {
                            for (int i = start; i < end; i++)
                            {
                                ret.Add(list[i]);
                            }
                        }
                    }
                }
                return ret;
            }
        }
        public static Game GetGame(int id)
        {
            return Games.Find(g => g.Id == id);
        }
        public static Client GetClient(string username)
        {
            lock (clientsLocker)
            {
                return Clients.Find(c => c.Username.Equals(username));
            }
        }
        public static List<Client> GetClients()
        {
            lock (clientsLocker)
            {
                return Clients;
            }
        }
        public static List<Client> GetClientsPaged(int page, int pageSize)
        {
            lock (clientsLocker)
            {
                List<Client> ret = new List<Client>();
                if (pageSize > 0 && page > 0)
                {
                    int count = Clients.Count;
                    int start = page * pageSize - pageSize;
                    int end = Math.Min(start + pageSize, count);
                    if (start >= 0 && start < count)
                    {
                        for (int i = start; i < end; i++)
                        {
                            ret.Add(Clients[i]);
                        }
                    }
                }
                return ret;
            }
        }
        public static bool AddClient(string username)
        {
            lock (clientsLocker)
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
            lock (clientsLocker)
            {
                Client c = GetClient(username);
                Clients.Remove(c);
            }
        }
        public static int GetNewId()
        {
            lock (gamesLocker)
            {
                return IdCounter++;
            }
        }

        public static void AddGame(Game game)
        {
            lock (gamesLocker)
            {
                game.Id = GetNewId();
                Games.Add(game);
            }
        }
        public static void DeleteGame(Game game)
        {
            lock (gamesLocker)
            {
                Games.Remove(game);
                lock (clientsLocker)
                {
                    foreach (Client c in Clients)
                    {
                        c.OwnedGames.Remove(game.Id);
                    }
                }
            }
        }
        public static bool BuyGame(string username, int id)
        {
            bool ret = false;
            Client client = GetClient(username);
            lock (gamesLocker)
            {
                if (Games.Find(g => g.Id == id) != null)
                {
                    if (!client.OwnedGames.Contains(id))
                    {
                        client.OwnedGames.Add(id);
                        ret = true;
                    }
                }
            }
            return ret;
        }
        public static bool RemoveGameFromClient(string username, int id)
        {
            bool ret = false;
            Client client = GetClient(username);
            ret = client.OwnedGames.Remove(id);
            return ret;
        }
        public static void RemoveAllGamesFromClient(string username)
        {
            Client c = GetClient(username);
            c.OwnedGames.Clear();
        }
        public static bool ReplaceGame(Game game)
        {
            lock (gamesLocker)
            {
                for (int i = 0; i < Games.Count; i++)
                {
                    if (Games[i].Equals(game))
                    {
                        Games[i] = game;
                        return true;
                    }
                }
                return false;
            }
        }
        public static bool AddReview(int id, Review review)
        {
            lock (gamesLocker)
            {
                Game g = Games.Find(g => g.Id == id);
                if (g != null)
                {
                    g.Reviews.Add(review);
                    return true;
                }
                return false;
            }
        }
    }
}
