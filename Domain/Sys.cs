﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Domain
{
    public static class Sys
    {
        public static int IdCounter { get; set; }
        public static List<Game> Games { get; set; }
        public static List<Client> Clients { get; set; }

        static Sys()
        {
            Games = new List<Game>();
            Clients = new List<Client>();
        }
        public static int GetNewId()
        {
            return IdCounter++;
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
        public static void ReplaceGame(Game game)
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
        public static void AddReview(int id, Review review)
        {
            Game g = Games.Find(g => g.Id == id);
            if (g != null)
            {
                g.Reviews.Add(review);
            }

        }
    }
}
