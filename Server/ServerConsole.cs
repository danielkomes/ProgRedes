using System;
using System.Collections.Generic;
using System.IO;
using Domain;

namespace Server
{
    public class ServerConsole
    {
        private const string IncorrectInputError = "Incorrect input";
        private Game GameToPublish { get; set; }
        private Game GameToView { get; set; }
        private Review Review { get; set; }
        private readonly ServerHandler sh;

        public ServerConsole(ServerHandler sh)
        {
            this.sh = sh;
            Menu0();
        }
        private void Menu0()
        {
            bool loop = true;
            while (loop)
            {
                string options = "1 Publish game\r\n" +
                    "2 Find game\r\n" +
                    "3 Shutdown server\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    MenuPublishGame();
                }
                else if (option == 2)
                {
                    FilterGameByAttr();
                }
                else if (option == 3)
                {
                    sh.CloseConnection();
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private void MenuPublishGame()
        {
            bool loop = true;
            while (loop)
            {
                if (GameToPublish == null)
                {
                    GameToPublish = new Game();
                }
                string options = "\r\n------------\r\n1 Title: " + GameToPublish.Title + "\r\n" +
                    "2 Genre: " + GameToPublish.Genre + "\r\n" +
                    "3 Age rating: " + GameToPublish.AgeRating + "\r\n" +
                    "4 Description: " + GameToPublish.Description + "\r\n" +
                    "5 Poster: " + GameToPublish.Poster + "\r\n" +
                    "\r\n" +
                    "6 Accept\r\n" +
                    "7 Back\r\n" +
                    "8-------------- TEST FILL\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    options = "\r\nInput name:\r\n";
                    Console.WriteLine(options);
                    GameToPublish.Title = Console.ReadLine();
                }
                else if (option == 2)
                {
                    GameToPublish.Genre = MenuGenres();
                }
                else if (option == 3)
                {
                    MenuAgeRating();
                }
                else if (option == 4)
                {
                    MenuDescription();
                }
                else if (option == 5)
                {
                    MenuPoster();
                }
                else if (option == 6) //Accept
                {
                    MenuAcceptPublishGame();
                }
                else if (option == 7)
                {
                    GameToPublish = null;
                    loop = false;
                }
                else if (option == 8)
                {
                    GameToPublish.Title = "TEST TITLE";
                    GameToPublish.Genre = EGenre.Action;
                    GameToPublish.AgeRating = 999;
                    GameToPublish.Description = "TEST DESCRIPTION";
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        #region Publish game
        private EGenre MenuGenres()
        {
            bool loop = true;
            EGenre ret = EGenre.None;
            while (loop)
            {
                string options = "Input genre: \r\n" +
                    "1 " + EGenre.Action + "\r\n" +
                    "2 " + EGenre.Adventure + "\r\n" +
                    "3 " + EGenre.Horror + "\r\n" +
                    "4 " + EGenre.Survival + "\r\n" +
                    "5 " + EGenre.RPG + "\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    ret = EGenre.Action;
                    loop = false;
                }
                else if (option == 2)
                {
                    ret = EGenre.Adventure;
                    loop = false;
                }
                else if (option == 3)
                {
                    ret = EGenre.Horror;
                    loop = false;
                }
                else if (option == 4)
                {
                    ret = EGenre.Survival;
                    loop = false;
                }
                else if (option == 5)
                {
                    ret = EGenre.RPG;
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
            return ret;
        }
        private void MenuAgeRating()
        {
            bool loop = true;
            while (loop)
            {
                string options = "Input age rating: ";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option >= 0)
                {
                    GameToPublish.AgeRating = option;
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private void MenuDescription()
        {
            string options = "Input description: ";
            Console.WriteLine(options);
            string input = Console.ReadLine();
            GameToPublish.Description = input;
        }
        private void MenuPoster()
        {
            string options = "Input poster path: ";
            Console.WriteLine(options);
            string input = Console.ReadLine();
            if (File.Exists(input))
            {
                GameToPublish.Poster = input;
            }
            else
            {
                Console.WriteLine("Path not found");
            }
        }
        private void MenuAcceptPublishGame()
        {
            if (GameToPublish.IsFieldsFilled())
            {
                Sys.AddGame(GameToPublish);
                GameToPublish = null;
                Console.WriteLine("Done. Game published");
            }
            else
            {
                Console.WriteLine("Some fields are missing info");
            }
        }

        #endregion
        #region View game
        private void FilterGameByAttr()
        {
            bool loop = true;
            while (loop)
            {
                string options = "\r\n1 Filter by title\r\n" +
                    "2 Filter by genre\r\n" +
                    "3 Filter by rating\r\n" +
                    "4 Back\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    FilterByTitle();
                }
                else if (option == 2)
                {
                    FilterByGenre();
                }
                else if (option == 3)
                {
                    FilterByRating();
                }
                else if (option == 4)
                {
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private void FilterByTitle()
        {
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("Input title to search: ");
                string input = Console.ReadLine();
                List<Game> filteredList = Logic.FilterByTitle(Sys.Games, input);
                int sel = SelectGame(filteredList);
                if (sel == -1)
                {
                    loop = false;
                }
            }
        }
        private void FilterByGenre()
        {
            bool loop = true;
            while (loop)
            {
                EGenre genre = MenuGenres();
                List<Game> filteredList = Logic.FilterByGenre(Sys.Games, genre);
                int sel = SelectGame(filteredList);

                if (sel == -1)
                {
                    loop = false;
                }
            }
        }
        private void FilterByRating()
        {
            bool loop = true;
            while (loop)
            {
                string options = "1 0-25\r\n" +
                    "2 26-50\r\n" +
                    "3 51-75\r\n" +
                    "4 76-100\r\n" +
                    "5 Back\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                List<Game> filteredList = null;
                if (option == 1)
                {
                    filteredList = Logic.FilterByRating(Sys.Games, 0, 25);
                }
                else if (option == 2)
                {
                    filteredList = Logic.FilterByRating(Sys.Games, 26, 50);
                }
                else if (option == 3)
                {
                    filteredList = Logic.FilterByRating(Sys.Games, 51, 75);
                }
                else if (option == 4)
                {
                    filteredList = Logic.FilterByRating(Sys.Games, 76, 100);
                }
                else if (option == 5)
                {
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
                if (filteredList != null)
                {
                    SelectGame(filteredList);
                }
            }
        }
        private int SelectGame(List<Game> list)
        {
            bool loop = true;
            int ret = 0;
            while (loop)
            {
                ret = 0;
                string options = "\r\n---------\r\n" +
                    "1 Back\r\n" +
                    "----------\r\n";
                options += Logic.ListGames(list);
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    loop = false;
                    ret = -1;
                }
                else if (option >= 0)
                {
                    GameToView = Logic.GetGameByIndex(option, list);
                    if (GameToView != null)
                    {
                        ViewGame();
                        loop = false;
                    }
                    else
                    {
                        Console.WriteLine(IncorrectInputError);
                    }
                }
            }
            return ret;
        }
        private void ViewGame()
        {
            bool loop = true;
            while (loop && GameToView != null)
            {
                string options = "\r\n-------\r\n" +
                    "Viewing game: " + GameToView.Title +
                    "\r\n-------\r\n" +
                    "1 Edit game\r\n" +
                    "2 Review game\r\n" +
                    "3 Details\r\n" +
                    "4 Back\r\n" +
                    "----------------\r\n";

                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    EditGame();
                }
                else if (option == 2)
                {
                    ReviewGame();
                }
                else if (option == 3)
                {
                    DetailsGame();
                }
                else if (option == 4)
                {
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        #endregion
        #region Edit game
        private void EditGame()
        {
            bool loop = true;
            while (loop && GameToView != null)
            {
                string options = "\r\n-------\r\n" +
                    "Edit game: " + GameToView.Title +
                    "\r\n-------\r\n" +
                    "1 Edit title\r\n" +
                    "2 Edit age rating\r\n" +
                    "3 Edit description\r\n" +
                    "4 Delete game\r\n" +
                    "5 Accept\r\n" +
                    "6 Back\r\n" +
                    "----------------\r\n";

                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    MenuEditTitle();
                }
                else if (option == 2)
                {
                    MenuEditAgeRating();
                }
                else if (option == 3)
                {
                    MenuEditDescription();
                }
                else if (option == 4)
                {
                    MenuConfirmDeleteGame();
                }
                else if (option == 5)
                {
                    loop = false;
                }
                else if (option == 6)
                {
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private void MenuEditTitle()
        {
            Console.WriteLine("Input new title: ");
            string newTitle = Console.ReadLine();
            GameToView.Title = newTitle;
        }
        private void MenuEditAgeRating()
        {
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("Input new age rating: ");
                string inputNewAgeRating = Console.ReadLine();
                int newAgeRating = GetOption(inputNewAgeRating);
                if (newAgeRating >= 0)
                {
                    GameToView.AgeRating = newAgeRating;
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private void MenuEditDescription()
        {
            Console.WriteLine("Input new description: ");
            string newDescription = Console.ReadLine();
            GameToView.Description = newDescription;
        }
        private bool MenuConfirmDeleteGame()
        {
            bool ret = false;
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("Are you sure you want to delete game: '" + GameToView.Title + "'?  yes/no");
                string input = Console.ReadLine();
                if (input.Equals("yes"))
                {
                    Sys.DeleteGame(GameToView);
                    GameToView = null;
                    loop = false;
                    ret = true;
                }
                else if (input.Equals("no"))
                {
                    loop = false;
                    ret = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
            return ret;
        }
        #endregion
        #region Review game
        private void ReviewGame()
        {
            bool loop = true;
            while (loop && GameToView != null)
            {
                if (Review == null)
                {
                    Review = new Review();
                }
                string currentReview = Review.Description;
                string currentRating = Review.Rating + "";
                if (string.IsNullOrEmpty(currentReview))
                {
                    currentReview = "None";
                }
                if (currentRating.Equals("0"))
                {
                    currentRating = "None";
                }

                string options = "\r\n-------\r\n" +
                    "Reviewing game: " + GameToView.Title +
                    "\r\n-------\r\n" +
                    "1 Write review (current: " + currentReview + ")\r\n" +
                    "2 Rate game (current: " + currentRating + ")\r\n" +
                    "3 Accept\r\n" +
                    "4 Back\r\n" +
                    "----------------\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    WriteReview();
                }
                else if (option == 2)
                {
                    RateGame();
                }
                else if (option == 3)
                {
                    AcceptReview();
                }
                else if (option == 4) //back
                {
                    Review = null;
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private void WriteReview()
        {
            Console.WriteLine("Input review for '" + GameToView.Title + "'");
            Review.Description = Console.ReadLine();
        }
        private void RateGame()
        {
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("Input rating for '" + GameToView.Title + "' in a scale from 1 (worst) to 100 (best)");
                string input = Console.ReadLine();
                int rating = GetOption(input);
                if (rating >= 1 && rating <= 100)
                {
                    Review.Rating = rating;
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private void AcceptReview()
        {
            if (Review.IsFieldsFilled())
            {
                Sys.AddReview(GameToView.Id, Review);
                Review = null;
                Console.WriteLine("Review posted");
            }
            else
            {
                Console.WriteLine("\r\nSome fields are missing info\r\n");
            }
        }
        #endregion
        #region Details game
        private void DetailsGame()
        {
            bool loop = true;
            while (loop)
            {
                string options = "\r\n-------\r\n" +
                    "Viewing game: " + GameToView.Title +
                    "\r\n-------\r\n" +
                    "Title: " + GameToView.Title + "\r\n" +
                    "Genre: " + GameToView.Genre + "\r\n" +
                    "Age rating: " + GameToView.AgeRating + "\r\n" +
                    "Description: " + GameToView.Description + "\r\n" +
                    "Poster: " + GameToView.Poster + "\r\n" +
                    "\r\n---------\r\n" +
                    "1 See reviews\r\n" +
                    "2 Back\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    SeeReviews();
                }
                else if (option == 2)
                {
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private void SeeReviews()
        {
            bool loop = true;
            while (loop)
            {
                string options = "\r\n-------\r\n" +
                    "Viewing reviews for: " + GameToView.Title + "\r\n" +
                    "Average rating: " + GameToView.AverageRating() +
                    "\r\n-------\r\n" +
                    Logic.ListReviews(GameToView) + "\r\n" +
                    "1 Back\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        #endregion
        private int GetOption(string input)
        {
            int option = -1;
            try
            {
                option = int.Parse(input);
            }
            catch (Exception)
            {
            }
            return option;
        }
    }

}
