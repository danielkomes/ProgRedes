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
                string options = "1 Shutdown server\r\n";
                Console.WriteLine(options);
                string input = Console.ReadLine();
                int option = GetOption(input);
                if (option == 1)
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
