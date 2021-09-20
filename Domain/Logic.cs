using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    static class Logic
    {
        static void Main(string[] args)
        {
            TestGames();
        }

        private static void TestGames()
        {
            Game batman = new Game
            {
                Title = "Batman",
                Genre = EGenre.Action,
                AgeRating = 18,
                Description = "mentally unstable man dresses up at nights, beats up people into near comma state, lets other people die in his quest to never kill anyone",
                Caratula = "batman caratula"
            };
            Sys.AddGame(batman);
            Game warframe = new Game
            {
                Title = "Warframe",
                Genre = EGenre.Action,
                AgeRating = 18,
                Description = "children with ninja complex with dysfunctional family find another dysfunctional family, fix it and get adopted by them, and then try to kill their original family",
                Caratula = "warframe caratula"
            };
            Sys.AddGame(warframe);

            Game mario = new Game
            {
                Title = "Mario Oddysey",
                Genre = EGenre.Adventure,
                AgeRating = 7,
                Description = "plumber with dead carreer almost gets killed, is saved by alien who then helps him overtake the mind of creatures against their will, all to get a girl",
                Caratula = "mario caratula"
            };
            Sys.AddGame(mario);
            Game kirby = new Game
            {
                Title = "Kirby",
                Genre = EGenre.Adventure,
                AgeRating = 7,
                Description = "amorphous god lands on planet and then eats its inhabitants whole",
                Caratula = "kirby caratula"
            };
            Sys.AddGame(kirby);

            Game bubsy = new Game
            {
                Title = "Bubsy",
                Genre = EGenre.Horror,
                AgeRating = 3,
                Description = "mutant feline-flying squirrel trying too hard to be funny gets teleported to different universes (i guess?) where almost every mundane thing is deadly",
                Caratula = "bubsy caratula"
            };
            Sys.AddGame(bubsy);
            Game amnesia = new Game
            {
                Title = "Amnesia",
                Genre = EGenre.Horror,
                AgeRating = 18,
                Description = "i can't remember",
                Caratula = "amnesia caratula"
            };
            Sys.AddGame(amnesia);

            Game minecraft = new Game
            {
                Title = "Minecraft",
                Genre = EGenre.Survival,
                AgeRating = 7,
                Description = "alien humanoids appear in a world and proceed to exhaust natural resources and ravage the land, and then kill the last member of a protected species",
                Caratula = "minecraft caratula"
            };
            Sys.AddGame(minecraft);
            Game terraria = new Game
            {
                Title = "Terraria",
                Genre = EGenre.Survival,
                AgeRating = 12,
                Description = "minecraft but 2d and more fun and less feelings of regret",
                Caratula = "terraria caratula"
            };
            Sys.AddGame(terraria);

            Game gta = new Game
            {
                Title = "GTA V",
                Genre = EGenre.RPG,
                AgeRating = 18,
                Description = "2 psychopaths with plot armor ruin even more a young black man's future",
                Caratula = "gta caratula"
            };
            Sys.AddGame(gta);
            Game tera = new Game
            {
                Title = "Tera",
                Genre = EGenre.RPG,
                AgeRating = 12,
                Description = "experience getting catfished by men playing as girls in unnecesarily revealing outfits at 20fps because your gpu is busy calculating all the 'jiggle' physics",
                Caratula = "tera caratula"
            };
            Sys.AddGame(tera);
        }

        public static string ListGames()
        {
            string ret = "";
            foreach (Game game in Sys.Games)
            {
                ret += game.Title + "\r\n";
            }
            return ret;
        }
    }
}
