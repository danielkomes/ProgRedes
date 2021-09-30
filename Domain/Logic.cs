using System;
using System.Collections.Generic;

namespace Domain
{
    public static class Logic
    {
        private const int indexStart = 2;
        public const string GameTransferSeparator = "%";
        public const string ReviewTransferSeparator = "$";
        public const string GameSeparator = "~";

        static void Main(string[] args)
        {
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
                Title = "Mario Oddyssey",
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

        public static List<Game> FilterByTitle(List<Game> list, string title)
        {
            List<Game> ret = new List<Game>();
            foreach (Game g in list)
            {
                if (g.Title.Contains(title))
                {
                    ret.Add(g);
                }
            }
            return ret;
        }
        public static List<Game> FilterByGenre(List<Game> list, EGenre genre)
        {
            List<Game> ret = new List<Game>();
            foreach (Game g in list)
            {
                if (g.Genre.Equals(genre))
                {
                    ret.Add(g);
                }
            }
            return ret;
        }
        public static List<Game> FilterByRating(List<Game> list, int min, int max)
        {
            List<Game> ret = new List<Game>();
            foreach (Game g in list)
            {
                if (g.AverageRating() >= min && g.AverageRating() <= max)
                {
                    ret.Add(g);
                }
            }
            return ret;
        }

        public static string ListGames(List<Game> list)
        {
            string ret = "";
            int index = indexStart; //para listar, opción 1 será Back, los juegos van de la 2 en adelante
            foreach (Game game in list)
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

        public static Game GetGameByIndex(int input, List<Game> list)
        {
            int index = input - indexStart;
            Game ret = null;
            if (index >= 0 && index < list.Count)
            {
                ret = list[index];
            }
            return ret;
        }
        public static string EncodeOwnedGames(List<int> list)
        {
            string ret = "";
            for (int i = 0; i < list.Count; i++)
            {
                ret += list[i];
                if (i < list.Count - 1)
                {
                    ret += GameSeparator;
                }
            }
            return ret;
        }
        public static List<int> DecodeOwnedGames(string s)
        {
            List<int> ret = new List<int>();
            string[] arr = s.Split(GameSeparator);
            foreach (string game in arr)
            {
                if (!string.IsNullOrEmpty(game))
                {
                    ret.Add(int.Parse(game));
                }
            }
            return ret;
        }
        public static string EncodeListGames(List<Game> list)
        {
            string ret = "";
            foreach (Game g in list)
            {
                ret += EncodeGame(g);
                if (list.IndexOf(g) < list.Count - 1)
                {
                    ret += GameSeparator;
                }
            }
            return ret;
        }
        public static string EncodeGame(Game game)
        {
            return game.Id + GameTransferSeparator +
                game.Title + GameTransferSeparator +
                game.Genre + GameTransferSeparator +
                game.AgeRating + GameTransferSeparator +
                game.Description + GameTransferSeparator +
                EncodeReviews(game.Reviews);
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
        public static string EncodeReview(Review r)
        {
            return r.Description + ReviewTransferSeparator +
                r.Rating;
        }

        public static List<Game> DecodeListGames(string s)
        {
            List<Game> ret = new List<Game>();
            if (s.Length > 0)
            {
                string[] arr = s.Split(GameSeparator);
                foreach (string game in arr)
                {
                    ret.Add(DecodeGame(game));
                }
            }
            return ret;
        }

        public static Game DecodeGame(string s)
        {
            string[] arr = s.Split(GameTransferSeparator);
            Game ret = new Game(int.Parse(arr[0]))
            {
                Title = arr[1],
                Genre = DecodeGenre(arr[2]),
                AgeRating = int.Parse(arr[3]),
                Description = arr[4],
                Reviews = DecodeReviews(arr[5]),
            };
            return ret;
        }
        private static EGenre DecodeGenre(string s)
        {
            EGenre ret = EGenre.None;
            foreach (var genre in Enum.GetValues(typeof(EGenre)))
            {
                if (s.Equals(genre.ToString()))
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
                    Review r = DecodeReview(arr[i] + ReviewTransferSeparator + arr[i + 1]);
                    ret.Add(r);
                }
            }
            return ret;
        }
        public static Review DecodeReview(string s)
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
