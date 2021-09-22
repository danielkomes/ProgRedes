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

        public void AddReview(Review r)
        {
            Reviews.Add(r);
        }

        public double AverageRating()
        {
            int sum = 0;
            foreach (Review r in Reviews)
            {
                sum += r.Rating;
            }
            int ret;
            if (Reviews.Count == 0)
            {
                ret = 0;
            }
            else
            {
                ret = sum / Reviews.Count;
            }
            return ret;
        }

        public bool IsFieldsFilled()
        {
            return !string.IsNullOrEmpty(Title) && Genre != EGenre.None && AgeRating != 0 && !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Poster);
        }
    }
}
