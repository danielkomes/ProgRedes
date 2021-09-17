using System;
using System.Collections.Generic;

namespace Domain
{

    class Game
    {
        public string Title { get; set; }
        public EGenre Genre { get; set; }
        public int AgeRating { get; set; } //podria ser un enum
        public string Description { get; set; }
        //public object? caratula??
        public List<Review> Reviews { get; set; }
    }
}
