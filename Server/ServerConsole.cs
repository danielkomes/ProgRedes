using System;
using System.Collections.Generic;
using System.IO;
using Domain;

namespace Server
{
    public class ServerConsole
    {
        private const string IncorrectInputError = "Incorrect input";
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
                string options = "1 Shutdown server\r\n" +
                    "2 Add user account\r\n" +
                    "3 Edit and delete accounts";
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
                    AddUserAccount();
                }
                else if (option == 3)
                {
                    EditAndDeleteUsers();
                }
                else
                {
                    Console.WriteLine(IncorrectInputError);
                }
            }
        }
        private void AddUserAccount()
        {
            Console.WriteLine("\r\nInput username for the new account");
            string input = Console.ReadLine();
            Client c = Sys.GetClient(input);
            if (c != null)
            {
                Console.WriteLine("Username already exists\r\n");
            }
            else
            {
                Sys.AddClient(input);
                Console.WriteLine("Account created\r\n");
            }
        }

        private void EditAndDeleteUsers()
        {
            bool loop = true;
            while (loop)
            {
                List<Client> list = new List<Client>(Sys.Clients);
                string options = "1 Back\r\n-----------\r\n" +
                    Logic.ListClients(list);
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
                {
                    loop = false;
                }
                else if (option != -1)
                {
                    Client c = Logic.GetClientByIndex(option, list);
                    if (c != null)
                    {
                        UserDetails(c);
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

        private void UserDetails(Client c)
        {
            bool loop = true;
            while (loop)
            {
                string options = "-----Viewing user: " + c.Username + "-----\r\n" +
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
                    AddGameToUserAccount(c);
                }
                else if (option == 2)
                {
                    RemoveGameFromUserAccount(c);
                }
                else if (option == 3)
                {
                    RemoveAllGamesFromUserAccount(c);
                }
                else if (option == 4)
                {
                    loop = !DeleteUserAccount(c);
                }
                else if (option == 5)
                {
                    loop = false;
                }
            }
        }

        private void AddGameToUserAccount(Client c)
        {
            bool loop = true;
            while (loop)
            {
                List<Game> list = new List<Game>(Sys.Games);
                RemoveAlreadyOwnedGames(c, list);
                string options = "-----Adding game to user: " + c.Username + "-----\r\n" +
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
                        bool success = Sys.BuyGame(c.Username, g.Id);
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
        private void RemoveGameFromUserAccount(Client c)
        {
            bool loop = true;
            while (loop)
            {
                List<Game> list = new List<Game>();
                foreach (int id in c.OwnedGames)
                {
                    Game g = Sys.Games.FindLast(g => g.Id == id);
                    if (g != null)
                    {
                        list.Add(g);
                    }
                }
                string options = "-----Removing game from user: " + c.Username + "-----\r\n" +
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
                        Sys.RemoveGameFromClient(c.Username, g.Id);
                        Console.WriteLine("\r\n" + g.Title + " removed from " + c.Username + "user account\r\n");
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
        private void RemoveAllGamesFromUserAccount(Client c)
        {
            bool loop = true;
            while (loop)
            {
                string options = "\r\nALL " + c.OwnedGames.Count + " games will be removed from " + c.Username + " user account. Are you sure? (yes/no)\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                if (input.Equals("yes"))
                {
                    Sys.RemoveAllGamesFromClient(c.Username);
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
        private bool DeleteUserAccount(Client c)
        {
            bool ret = false;
            bool loop = true;
            while (loop)
            {
                string options = "Are you sure you want to delete " + c.Username + " user account? (yes/no)\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                if (input.Equals("yes"))
                {
                    Sys.DeleteClient(c.Username);
                    sh.KickClient(c);
                    Console.WriteLine(c.Username + " account deleted and client disconnected\r\n");
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

        private void RemoveAlreadyOwnedGames(Client c, List<Game> list)
        {
            foreach (int gameId in c.OwnedGames)
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
