using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Domain;

namespace Client
{
    public class ClientConsole
    {
        private const string IncorrectInputError = "Incorrect input";
        private Game GameToPublish { get; set; }
        private List<Game> ListGames { get; set; }
        private List<int> ListOwnedGames { get; set; }
        private Game GameToView { get; set; }
        private Review Review { get; set; }
        private Domain.Client Client { get; set; }
        private readonly ClientHandler ch;

        private ClientConsole(ClientHandler ch)
        {
            this.ch = ch;
        }

        public static async Task<ClientConsole> ClientConsoleAsync(ClientHandler ch)
        {
            ClientConsole cc = new ClientConsole(ch);
            try
            {
                await cc.CredentialsMenuAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("The connection has been lost.\r\nPress Enter to close the console");
                Console.ReadLine();
            }
            return cc;
        }

        private async Task RequestListGamesAsync()
        {
            await ch.SendMessageAsync(ETransferType.List, "");
            string list = await ch.ReceiveMessageAsync();
            ListGames = Logic.DecodeListGames(list);
        }
        private async Task RequestOwnedGamesAsync()
        {
            await ch.SendMessageAsync(ETransferType.Owned, Client.Username);
            string list = await ch.ReceiveMessageAsync();
            ListOwnedGames = Logic.DecodeOwnedGames(list);
            Client.OwnedGames = ListOwnedGames;
        }
        private async Task<Game> UpdateGameAsync(Game g)
        {
            await RequestListGamesAsync();
            Game ret = null;
            try
            {
                ret = ListGames.Find(game => game.Id == g.Id);
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Can't find game. It may have been deleted");
            }
            return ret;
        }
        private async Task CredentialsMenuAsync()
        {
            bool loop = true;
            while (loop)
            {
                string options = "1 Login\r\n" +
                    "2 Sign up\r\n" +
                    "3 Disconnect\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    await LoginAsync();
                }
                else if (option == 2)
                {
                    await SignupAsync();
                }
                else if (option == 3)
                {
                    await ch.SendMessageAsync(ETransferType.Disconnect, "");
                    ch.CloseConnection();
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private async Task LoginAsync()
        {
            Console.WriteLine("Input username: ");
            string input = Console.ReadLine();
            await ch.SendMessageAsync(ETransferType.Login, input);
            string msg = await ch.ReceiveMessageAsync();
            bool success = bool.Parse(msg);
            if (success)
            {
                Client = new Domain.Client(input);
                await RequestOwnedGamesAsync();
                //Client.OwnedGames = Logic.DecodeOwnedGames(ch.ReceiveMessageAsync().Result);
                Console.WriteLine("Successfully logged in");
                await Menu0Async();
            }
            else
            {
                Console.WriteLine("Username not found or already logged in");
            }
        }
        private async Task SignupAsync()
        {
            Console.WriteLine("Input username: ");
            string input = Console.ReadLine();
            input = input.Trim();
            if (input.Equals(string.Empty))
            {
                Console.WriteLine("Username cannot be empty\r\n");
            }
            else
            {
                await ch.SendMessageAsync(ETransferType.Signup, input);
                string msg = await ch.ReceiveMessageAsync();
                bool success = bool.Parse(msg);
                if (success)
                {
                    Client = new Domain.Client(input);
                    Console.WriteLine("Successfully signed up");
                    await Menu0Async();
                }
                else
                {
                    Console.WriteLine("Username already exists");
                }
            }
        }
        private async Task Menu0Async()
        {
            bool loop = true;
            while (loop)
            {
                string options = "1 Publish game\r\n" +
                    "2 See owned games\r\n" +
                    "3 Find game\r\n" +
                    "4 Log off\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    await MenuPublishGameAsync();
                }
                else if (option == 2)
                {
                    await SeeOwnedGamesAsync();
                }
                else if (option == 3)
                {
                    await FilterGameByAttrAsync();
                }
                else if (option == 4)
                {
                    await ch.SendMessageAsync(ETransferType.Logoff, Client.Username);
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private async Task SeeOwnedGamesAsync()
        {
            bool loop = true;
            while (loop)
            {
                await RequestListGamesAsync();
                await RequestOwnedGamesAsync();
                List<Game> list = new List<Game>();
                foreach (int id in Client.OwnedGames)
                {
                    Game game = ListGames.Find(g => g.Id == id);
                    list.Add(game);
                }
                string options = "-----Viewing owned games-----\r\n" +
                    "---1 Back---\r\n" +
                    Logic.ListGames(list, false);
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
        #region Publish game
        private async Task MenuPublishGameAsync()
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
                    await MenuAcceptPublishGameAsync();
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
        private async Task MenuAcceptPublishGameAsync()
        {
            if (GameToPublish.IsFieldsFilled())
            {
                await ch.SendMessageAsync(ETransferType.Publish, Logic.EncodeGame(GameToPublish));
                Console.WriteLine("Sending. Please wait...");
                await ch.SendFileAsyncAsync(GameToPublish.Poster, GameToPublish.Id + ".jpg");
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
        private async Task FilterGameByAttrAsync()
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
                    await FilterByTitleAsync();
                }
                else if (option == 2)
                {
                    await FilterByGenreAsync();
                }
                else if (option == 3)
                {
                    await FilterByRatingAsync();
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
        private async Task FilterByTitleAsync()
        {
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("Input title to search: ");
                string input = Console.ReadLine();
                await RequestListGamesAsync();
                List<Game> filteredList = Logic.FilterByTitle(ListGames, input);
                int sel = await SelectGameAsync(filteredList);
                if (sel == -1)
                {
                    loop = false;
                }
            }
        }
        private async Task FilterByGenreAsync()
        {
            bool loop = true;
            while (loop)
            {
                EGenre genre = MenuGenres();
                await RequestListGamesAsync();
                List<Game> filteredList = Logic.FilterByGenre(ListGames, genre);
                int sel = await SelectGameAsync(filteredList);

                if (sel == -1)
                {
                    loop = false;
                }
            }
        }
        private async Task FilterByRatingAsync()
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
                await RequestListGamesAsync();
                List<Game> filteredList = null;
                if (option == 1)
                {
                    filteredList = Logic.FilterByRating(ListGames, 0, 25);
                }
                else if (option == 2)
                {
                    filteredList = Logic.FilterByRating(ListGames, 26, 50);
                }
                else if (option == 3)
                {
                    filteredList = Logic.FilterByRating(ListGames, 51, 75);
                }
                else if (option == 4)
                {
                    filteredList = Logic.FilterByRating(ListGames, 76, 100);
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
                    await SelectGameAsync(filteredList);
                }
            }
        }
        private async Task<int> SelectGameAsync(List<Game> list)
        {
            bool loop = true;
            int ret = 0;
            while (loop)
            {
                await RequestListGamesAsync();
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
                        await ViewGameAsync();
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
        private async Task ViewGameAsync()
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
                    await EditGameAsync();
                }
                else if (option == 2)
                {
                    await ReviewGameAsync();
                }
                else if (option == 3)
                {
                    await DetailsGameAsync();
                }
                else if (option == 4)
                {
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
                GameToView = await UpdateGameAsync(GameToView);
            }
        }
        #endregion
        #region Edit game
        private async Task EditGameAsync()
        {
            bool loop = true;
            while (loop && GameToView != null)
            {
                string options = "\r\n-------\r\n" +
                     "Edit game: " + GameToView.Title +
                     "\r\n-------\r\n" +
                     "1 Edit title (current: " + GameToView.Title + ")\r\n" +
                     "2 Edit age rating (current: " + GameToView.AgeRating + ")\r\n" +
                     "3 Edit description (current: " + GameToView.Description + ")\r\n" +
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
                    await MenuConfirmDeleteGame();
                }
                else if (option == 5)
                {
                    await ch.SendMessageAsync(ETransferType.Edit, Logic.EncodeGame(GameToView));
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
        private async Task<bool> MenuConfirmDeleteGame()
        {
            bool ret = false;
            bool loop = true;
            while (loop)
            {
                Console.WriteLine("Are you sure you want to delete game: '" + GameToView.Title + "'?  yes/no");
                string input = Console.ReadLine();
                if (input.Equals("yes"))
                {
                    await ch.SendMessageAsync(ETransferType.Delete, Logic.EncodeGame(GameToView));
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
        private async Task ReviewGameAsync()
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
                    await AcceptReviewAsync();
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
        private async Task AcceptReviewAsync()
        {
            if (Review.IsFieldsFilled())
            {
                await ch.SendMessageAsync(ETransferType.Review, GameToView.Id + Logic.GameTransferSeparator + Logic.EncodeReview(Review));
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
        private async Task DetailsGameAsync()
        {
            bool loop = true;
            while (loop && GameToView != null)
            {
                string options = "\r\n-------\r\n" +
                    "Viewing game: " + GameToView.Title +
                    "\r\n-------\r\n" +
                    "Title: " + GameToView.Title + "\r\n" +
                    "Genre: " + GameToView.Genre + "\r\n" +
                    "Age rating: " + GameToView.AgeRating + "\r\n" +
                    "Description: " + GameToView.Description + "\r\n" +
                    "\r\n---------\r\n" +
                    "1 Download poster\r\n" +
                    "2 Get game\r\n" +
                    "3 See reviews\r\n" +
                    "4 Back\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    await ch.SendMessageAsync(ETransferType.Download, Logic.EncodeGame(GameToView));
                    await ch.ReceiveFileAsync();
                    Console.WriteLine("\r\n------Poster downloaded------\r\n");
                }
                else if (option == 2)
                {
                    await ch.SendMessageAsync(ETransferType.BuyGame, GameToView.Id + Logic.GameTransferSeparator + Client.Username);
                    string msg = await ch.ReceiveMessageAsync();
                    bool response = bool.Parse(msg);
                    if (response)
                    {
                        Console.WriteLine("\r\n------Game bought------\r\n");
                        await RequestOwnedGamesAsync();
                        //Client.OwnedGames = Logic.DecodeOwnedGames(ch.ReceiveMessageAsync().Result);
                    }
                    else
                    {
                        Console.WriteLine("Could not buy game. You might already own it or it may have been deleted");
                    }
                }
                else if (option == 3)
                {
                    SeeReviews();
                }
                else if (option == 4)
                {
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
                GameToView = await UpdateGameAsync(GameToView);
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
