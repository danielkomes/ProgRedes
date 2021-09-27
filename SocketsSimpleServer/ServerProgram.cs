using Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class ServerProgram
    {
        private static List<Socket> clients;
        private static ServerHandler sh;

        static void Main(string[] args)
        {
            Logic.TestGames();
            sh = new ServerHandler();
            clients = new List<Socket>();
        }
    }
}
