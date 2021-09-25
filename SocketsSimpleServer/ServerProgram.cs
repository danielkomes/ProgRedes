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
        //private static Socket serverSocket;
        //private const string ServerIpAddress = "127.0.0.1";
        //private const int ServerPort = 6000;
        //private const int Backlog = 4;
        //private const int FixedSize = 4;
        private static List<Socket> clients;
        private static ServerHandler sh;
        //private static ClientConsoleOld console;

        static void Main(string[] args)
        {
            sh = new ServerHandler();
            clients = new List<Socket>();
        }
        static void ResponseToClient(Socket clientSocket)
        {
            while (true)
            {
                // 1 Leo el mensaje
                string message = Console.ReadLine();
                //sh.SendMessage(message);
            }
        }


    }
}
