using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ClientConsole
    {
        private const string IncorrectInputError = "Incorrect input";
        private Game GameToPublish { get; set; }
        private Game GameToView { get; set; }
        private Review Review { get; set; }

        public ClientConsole()
        {
            Menu0();
        }
        public void GetConsole()
        {
            string input = "";
            string options = "";
            int selection = -1;
            bool acceptInput = false;

            while (!acceptInput)
            {
                options = "1 Publish game\r\n" +
                    "2 Find game\r\n";
                input = Console.ReadLine();
                selection = GetOption(input);
            }
            if (selection == 1)
            {
                if (GameToPublish == null)
                {
                    GameToPublish = new Game();
                }

                options = "1 Title: " + GameToPublish.Title + "\r\n" +
                    "2 Genre: " + GameToPublish.Genre + "\r\n" +
                    "3 Age rating: " + GameToPublish.AgeRating + "\r\n" +
                    "4 Description: " + GameToPublish.Description + "\r\n" +
                    "5 Poster: " + GameToPublish.Poster + "\r\n" +
                    "\r\n" +
                    "6 Accept\r\n" +
                    "7 Back\r\n" +
                    "8-------------- TEST FILL\r\n";
            }
            else if (selection == 2)
            {

            }
            else
            {

            }
        } //old

        private void Menu0()
        {
            Logic.TestGames();
            while (true)
            {
                string options = "1 Publish game\r\n" +
                    "2 Find game\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    MenuPublishGame();
                }
                else if (option == 2)
                {
                    FindGame();
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
                string options = "";
                if (GameToPublish == null)
                {
                    GameToPublish = new Game();
                }
                options = "\r\n------------\r\n1 Title: " + GameToPublish.Title + "\r\n" +
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
                    MenuGenres();
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
                    if (GameToPublish.IsFieldsFilled())
                    {
                        Sys.AddGame(GameToPublish);
                        GameToPublish = null;
                        Console.WriteLine("Game published");
                    }
                    else
                    {
                        Console.WriteLine("Some fields are missing info");
                        MenuPublishGame();
                        return;
                    }
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
                    GameToPublish.Poster = "TEST POSTER";
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        #region Publish game
        private void MenuGenres()
        {
            bool loop = true;
            while (loop)
            {
                string options = "Ingresar genero: \r\n" +
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
                    GameToPublish.Genre = EGenre.Action;
                    loop = false;
                }
                else if (option == 2)
                {
                    GameToPublish.Genre = EGenre.Adventure;
                    loop = false;
                }
                else if (option == 3)
                {
                    GameToPublish.Genre = EGenre.Horror;
                    loop = false;
                }
                else if (option == 4)
                {
                    GameToPublish.Genre = EGenre.Survival;
                    loop = false;
                }
                else if (option == 5)
                {
                    GameToPublish.Genre = EGenre.RPG;
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
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
            string options = "Input poster: ";
            Console.WriteLine(options);
            string input = Console.ReadLine();
            GameToPublish.Poster = input;
        }

        #endregion
        private void FindGame()
        {
            bool loop = true;
            string options = "";
            while (loop)
            {
                options = "\r\n---------\r\n" +
                    "1 Back\r\n----------\r\n" +
                    Logic.ListGames();
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    loop = false;
                }
                else if (option >= 0)
                {
                    GameToView = Logic.GetGameByIndex(option);
                    ViewGame();
                }
            }
        }
        private void ViewGame()
        {
            bool loop = true;
            string options = "";
            while (loop && GameToView != null)
            {
                options = "\r\n-------\r\n" +
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
        #region Edit game
        private void EditGame()
        {
            bool loop = true;
            string options = "";
            while (loop && GameToView != null)
            {
                options = "\r\n-------\r\n" +
                    "Edit game: " + GameToView.Title +
                    "\r\n-------\r\n" +
                    "1 Edit title\r\n" +
                    "2 Edit description\r\n" +
                    "3 Edit age rating\r\n" +
                    "4 Delete game\r\n" +
                    "5 Back\r\n" +
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
            string options = "";
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

                options = "\r\n-------\r\n" +
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
                GameToView.AddReview(Review);
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
