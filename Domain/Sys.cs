using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    static class Sys
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
        }
    }
}
