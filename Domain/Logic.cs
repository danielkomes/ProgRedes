using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public static class Logic
    {
        private const int indexStart = 2;
        public const string GameTransferSeparator = "%";
        public const string ReviewTransferSeparator = "$";

        static void Main(string[] args)
        {
            TestGames();
        }

        public static void TestGames()
        {
            Game batman = new Game
            {
                Title = "Batman",
                Genre = EGenre.Action,
                AgeRating = 18,
                Description = "mentally unstable man dresses up at nights, beats up people into near comma state, lets other people die in his quest to never kill anyone",
                Poster = "batman poster"
            };
            Sys.AddGame(batman);
            Game warframe = new Game
            {
                Title = "Warframe",
                Genre = EGenre.Action,
                AgeRating = 18,
                Description = "children with ninja complex with dysfunctional family find another dysfunctional family, fix it and get adopted by them, and then try to kill their original family",
                Poster = "warframe poster"
            };
            Sys.AddGame(warframe);

            Game mario = new Game
            {
                Title = "Mario Oddysey",
                Genre = EGenre.Adventure,
                AgeRating = 7,
                Description = "plumber with dead carreer almost gets killed, is saved by alien who then helps him overtake the mind of creatures against their will, all to get a girl",
                Poster = "mario poster"
            };
            Sys.AddGame(mario);
            Game kirby = new Game
            {
                Title = "Kirby",
                Genre = EGenre.Adventure,
                AgeRating = 7,
                Description = "amorphous god lands on planet and then eats its inhabitants whole",
                Poster = "kirby poster"
            };
            Sys.AddGame(kirby);

            Game bubsy = new Game
            {
                Title = "Bubsy",
                Genre = EGenre.Horror,
                AgeRating = 3,
                Description = "mutant feline-flying squirrel trying too hard to be funny gets teleported to different universes (i guess?) where almost every mundane thing is deadly",
                Poster = "bubsy poster"
            };
            Sys.AddGame(bubsy);
            Game amnesia = new Game
            {
                Title = "Amnesia",
                Genre = EGenre.Horror,
                AgeRating = 18,
                Description = "i can't remember",
                Poster = "amnesia poster"
            };
            Sys.AddGame(amnesia);

            Game minecraft = new Game
            {
                Title = "Minecraft",
                Genre = EGenre.Survival,
                AgeRating = 7,
                Description = "alien humanoids appear in a world and proceed to exhaust natural resources and ravage the land, and then kill the last member of a protected species",
                Poster = "minecraft poster"
            };
            Sys.AddGame(minecraft);
            Game terraria = new Game
            {
                Title = "Terraria",
                Genre = EGenre.Survival,
                AgeRating = 12,
                Description = "minecraft but 2d and more fun and less feelings of regret",
                Poster = "terraria poster"
            };
            Sys.AddGame(terraria);

            Game gta = new Game
            {
                Title = "GTA V",
                Genre = EGenre.RPG,
                AgeRating = 18,
                Description = "2 psychopaths with plot armor ruin even more a young black man's future",
                Poster = "gta poster"
            };
            Sys.AddGame(gta);
            Game tera = new Game
            {
                Title = "Tera",
                Genre = EGenre.RPG,
                AgeRating = 12,
                Description = "experience getting catfished by men playing as girls in unnecesarily revealing outfits at 20fps because your gpu is busy calculating all the 'jiggle' physics",
                Poster = "tera poster"
            };
            Sys.AddGame(tera);
        }

        public static string ListGames()
        {
            string ret = "";
            int index = indexStart; //para listar, opción 1 será Back, los juegos van de la 2 en adelante
            foreach (Game game in Sys.Games)
            {
                ret += index + " " + game.Title + "\r\n";
                index++;
            }
            return ret;
        }

        public static string ListReviews(Game game)
        {
            string ret = "";
            foreach (Review r in game.Reviews)
            {
                ret += "\r\n" + r.Description + "\r\n" +
                    "Rating: " + r.Rating + "\r\n";
            }
            return ret;
        }

        public static Game GetGameByIndex(int input)
        {
            int index = input - indexStart;
            Game ret = null;
            if (index >= 0 && index < Sys.Games.Count)
            {
                ret = Sys.Games[index];
            }
            else
            {
                //out of range
            }
            return ret;
        }

        public static string EncodeGame(Game game)
        {
            return game.Id + GameTransferSeparator +
                game.Title + GameTransferSeparator +
                game.Genre + GameTransferSeparator +
                game.Description + GameTransferSeparator +
                ListReviews(game) + GameTransferSeparator +
                game.Poster;
        }

        public static string EncodeReviews(List<Review> l)
        {
            string ret = "";
            for (int i = 0; i < l.Count; i++)
            {
                ret += EncodeReview(l[i]);
                if (i < l.Count - 1)
                {
                    ret += ReviewTransferSeparator;
                }
            }
            return ret;
        }
        private static string EncodeReview(Review r)
        {
            return r.Description + ReviewTransferSeparator +
                r.Rating;
        }

        public static Game DecodeGame(string s)
        {
            string[] arr = s.Split(GameTransferSeparator);
            Game ret = new Game(int.Parse(arr[0]))
            {
                Title = arr[1],
                Genre = DecodeGenre(arr[2]),
                Description = arr[3],
                Reviews = DecodeReviews(arr[4]),
                Poster = arr[5],
            };
            return ret;
        }
        private static EGenre DecodeGenre(string s)
        {
            EGenre ret = EGenre.None;
            foreach (var genre in Enum.GetValues(typeof(EGenre)))
            {
                if (s.Equals(genre))
                {
                    ret = (EGenre)genre;
                }
            }
            return ret;
        }

        private static List<Review> DecodeReviews(string s)
        {
            List<Review> ret = new List<Review>();
            string[] arr = s.Split(ReviewTransferSeparator);
            if (!string.IsNullOrEmpty(s))
            {
                for (int i = 0; i < arr.Length; i += 2)
                {
                    Review r = DecodeReview(arr[0] + arr[1]);
                    ret.Add(r);
                }
            }
            return ret;
        }
        private static Review DecodeReview(string s)
        {
            string[] arr = s.Split(ReviewTransferSeparator);
            Review ret = new Review
            {
                Description = arr[0],
                Rating = int.Parse(arr[1])
            };
            return ret;
        }
    }
}
