using System;
using System.Collections.Generic;

namespace Domain
{

    public class Game
    {
        public string Title { get; set; }
        public EGenre Genre { get; set; }
        public int AgeRating { get; set; } //podria ser un enum
        public string Description { get; set; }
        public string Poster { get; set; }
        //public object? caratula??
        public List<Review> Reviews { get; set; }

        public Game()
        {
            Title = "";
            Genre = EGenre.None;
            Description = "";
            Reviews = new List<Review>();
            Poster = "";
        }

        public bool IsFieldsFilled()
        {
            return !string.IsNullOrEmpty(Title) && Genre != EGenre.None && AgeRating != 0 && !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Poster);
        }
    }
}
