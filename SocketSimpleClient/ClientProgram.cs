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
        //private const string ServerIpAddress = "127.0.0.1";
        //private const int ServerPort = 6000;
        //private const string ClientIpAddress = "127.0.0.1";
        //private const int ClientPort = 0;
        //private const int FixedSize = 4;
        private static ClientConsole ClientConsole;
        private static ClientHandler ClientHandler;
        //public static Socket clientSocket;


        static void Main(string[] args)
        {

            Console.WriteLine("Connected to server");
            ClientHandler = new ClientHandler();
            ClientConsole = new ClientConsole(ClientHandler);

            // esto es un ejemplo para mostrar el pasaje de datos
            // por ende es válido un while (true) para hacer más sencilla la tarea
            // el while (true) nunca sería válido en un trabajo que requiera corrección
            //while (true)
            //{
            //    // 1 Leo el mensaje
            //    string message = Console.ReadLine();
            //    ClientHandler.SendMessage(message);
            //}
        }

        static void Listen(Socket clientSocket)
        {
            while (true)
            {

                Console.WriteLine(ClientHandler.ReceiveMessage());
            }
        }
    }
}
