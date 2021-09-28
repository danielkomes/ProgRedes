using Common;
using Domain;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    public class ClientProgram
    {
        private static ClientConsole ClientConsole;
        private static ClientHandler ClientHandler;


        static void Main(string[] args)
        {
            ClientHandler = new ClientHandler();
            ClientConsole = new ClientConsole(ClientHandler);
        }

    }
}
