using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Domain
{
    public static class Sys
    {
        public static List<Game> Games { get; set; }
        public static List<Client> Clients { get; set; }

        static Sys()
        {
            Games = new List<Game>();
            Clients = new List<Client>();
        }

        public static void AddGame(Game game)
        {
            Games.Add(game);
            Console.WriteLine("new game: " + game.Title);
        }
        public static void DeleteGame(Game game)
        {
            Games.Remove(game);
        }

        private static void TestPrintGames()
        {
            while (true)
            {
                foreach (Game game in Games)
                {
                    Console.WriteLine(game.Title);
                }
            }
        }
    }
}
