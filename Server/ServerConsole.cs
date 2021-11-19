using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AdminServer;
using Domain;

namespace Server
{
    public class ServerConsole
    {
        private const string IncorrectInputError = "Incorrect input";
        private readonly ServerHandler sh;

        private ServerConsole(ServerHandler sh)
        {
            this.sh = sh;
        }
        public static async Task ServerConsoleAsync(ServerHandler sh)
        {
            ServerConsole sc = new ServerConsole(sh);
            foreach (string game in Logic.TestGamesEncoded())
            {
                await sh.PublishAsync("ADMIN",game);
            }
            await sc.Menu0();
        }
        private async Task Menu0()
        {
            bool loop = true;
            while (loop)
            {
                string options = "1 Shutdown server\r\n" +
                    "2 Add user account\r\n" +
                    "3 Edit and delete accounts\r\n" +
                    "4 View all games\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    sh.CloseConnection();
                    loop = false;
                }
                else if (option == 2)
                {
                    await AddUserAccount();
                }
                else if (option == 3)
                {
                    await EditAndDeleteUsers();
                }
                else if (option == 4)
                {
                    await ViewAllGames();
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private async Task AddUserAccount()
        {
            Console.WriteLine("\r\nInput username for the new account");
            string input = Console.ReadLine();
            //Client c = Sys.GetClient(input);
            MessageReply signupReply = await sh.SignupAsync(input);
            bool success = bool.Parse(signupReply.Message);
            if (!success)
            {
                Console.WriteLine("Username already exists\r\n");
            }
            else
            {
                //Sys.AddClient(input);
                await sh.LogoffAsync(input);
                Console.WriteLine("Account created\r\n");
            }
        }
        #region Edit and delete users
        private async Task EditAndDeleteUsers()
        {
            bool loop = true;
            while (loop)
            {
                //List<Client> list = new List<Client>(Sys.Clients);
                MessageReply listReply = await sh.ListClientsAsync("");
                List<string> userList = Logic.DecodeListClients(listReply.Message);

                string options = "1 Back\r\n-----------\r\n" +
                    Logic.ListClients(userList);
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    loop = false;
                }
                else if (option != -1)
                {
                    string c = Logic.GetClientByIndex(option, userList);
                    if (c != null)
                    {
                        await UserDetails(c);
                    }
                    else
                    {
                        Console.WriteLine(IncorrectInputError);
                    }
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private async Task UserDetails(string c)
        {
            bool loop = true;
            while (loop)
            {
                string options = "-----Viewing user: " + c + "-----\r\n" +
                    "1 Add game to user account\r\n" +
                    "2 Remove game from user account\r\n" +
                    "3 Remove all games from user account\r\n" +
                    "4 Delete user account\r\n" +
                    "5 Back\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    await AddGameToUserAccount(c);
                }
                else if (option == 2)
                {
                    await RemoveGameFromUserAccount(c);
                }
                else if (option == 3)
                {
                    await RemoveAllGamesFromUserAccount(c);
                }
                else if (option == 4)
                {
                    loop = !(await DeleteUserAccount(c));
                }
                else if (option == 5)
                {
                    loop = false;
                }
            }
        }
        private async Task AddGameToUserAccount(string c)
        {
            bool loop = true;
            while (loop)
            {
                //List<Game> list = new List<Game>(Sys.Games);
                MessageReply listReply = await sh.ListAsync("");
                List<Game> list = Logic.DecodeListGames(listReply.Message);
                await RemoveAlreadyOwnedGames(c, list);
                string options = "-----Adding game to user: " + c + "-----\r\n" +
                    "---1 Back---\r\n" +
                    Logic.ListGames(list);
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    loop = false;
                }
                else if (option != -1)
                {
                    Game g = Logic.GetGameByIndex(option, list);
                    if (g != null)
                    {
                        //bool success = Sys.BuyGame(c.Username, g.Id);
                        string req = g.Id + Logic.GameTransferSeparator + c;
                        MessageReply buyReply = await sh.BuyGameAsync(req);
                        bool success = bool.Parse(buyReply.Message);
                        if (!success)
                        {
                            Console.WriteLine("Could not add game to user account. It may have been deleted");
                        }
                    }
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private async Task RemoveGameFromUserAccount(string c)
        {
            bool loop = true;
            while (loop)
            {
                List<Game> list = new List<Game>();
                MessageReply gamesReply = await sh.ListAsync("");
                List<Game> games = Logic.DecodeListGames(gamesReply.Message);
                MessageReply ownedReply = await sh.OwnedAsync(c);
                List<int> owned = Logic.DecodeOwnedGames(ownedReply.Message);
                foreach (int id in owned)
                {
                    Game g = games.FindLast(g => g.Id == id);
                    if (g != null)
                    {
                        list.Add(g);
                    }
                }
                string options = "-----Removing game from user: " + c + "-----\r\n" +
                    "---1 Back---\r\n" +
                    Logic.ListGames(list);
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    loop = false;
                }
                else if (option != -1)
                {
                    Game g = Logic.GetGameByIndex(option, list);
                    if (g != null)
                    {
                        //Sys.RemoveGameFromClient(c, g.Id);
                        await sh.RemoveGameFromClientAsync(c, g.Id);
                        Console.WriteLine("\r\n" + g.Title + " removed from " + c + "user account\r\n");
                        loop = false;
                    }
                    else
                    {
                        Console.WriteLine(IncorrectInputError);
                    }
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private async Task RemoveAllGamesFromUserAccount(string c)
        {
            bool loop = true;
            while (loop)
            {
                MessageReply ownedReply = await sh.OwnedAsync(c);
                List<int> owned = Logic.DecodeOwnedGames(ownedReply.Message);

                string options = "\r\nALL " + owned.Count + " games will be removed from " + c + " user account. Are you sure? (yes/no)\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                if (input.Equals("yes"))
                {
                    //Sys.RemoveAllGamesFromClient(c);
                    await sh.RemoveAllGamesFromClientAsync(c);
                    Console.WriteLine("\r\nDone. All removed\r\n");
                    loop = false;
                }
                else if (input.Equals("no"))
                {
                    Console.WriteLine("Operation cancelled\r\n");
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private async Task<bool> DeleteUserAccount(string c)
        {
            bool ret = false;
            bool loop = true;
            while (loop)
            {
                string options = "Are you sure you want to delete " + c + " user account? (yes/no)\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                if (input.Equals("yes"))
                {
                    //Sys.DeleteClient(c);
                    await sh.DeleteClientAsync(c);
                    sh.KickClient(c);
                    Console.WriteLine(c + " account deleted and client disconnected\r\n");
                    loop = false;
                    ret = true;
                }
                else if (input.Equals("no"))
                {
                    Console.WriteLine("Operation cancelled\r\n");
                    loop = false;
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
            return ret;
        }
        #endregion
        private async Task ViewAllGames()
        {
            bool loop = true;
            while (loop)
            {
                MessageReply listReply = await sh.ListAsync("");
                List<Game> list = Logic.DecodeListGames(listReply.Message);
                string options = "----Viewing all games-----\r\n" +
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
        private async Task RemoveAlreadyOwnedGames(string c, List<Game> list)
        {
            MessageReply ownedReply = await sh.OwnedAsync(c);
            List<int> owned = Logic.DecodeOwnedGames(ownedReply.Message);
            foreach (int gameId in owned)
            {
                list.RemoveAll(g => g.Id == gameId);
            }
        }
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
