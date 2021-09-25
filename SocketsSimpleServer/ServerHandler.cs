using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common;
using Domain;

namespace Server
{
    public class ServerHandler
    {
        private readonly Socket serverSocket;
        private readonly IPEndPoint _serverIpEndPoint;
        public List<Socket> clients;

        public ServerHandler()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverIpEndPoint = new IPEndPoint(IPAddress.Loopback, 7000);
            serverSocket.Bind(_serverIpEndPoint);
            serverSocket.Listen(1);
            clients = new List<Socket>();
            AcceptClients();
        }
        void AcceptClients()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                clients.Add(clientSocket);
                Console.WriteLine("New client connected. Total: " + clients.Count);
                new Thread(() => Listen(clientSocket)).Start();
                //new Thread(() => ResponseToClient(clientSocket)).Start();
            }
        }
        void Listen(Socket clientSocket)
        {
            var fileCommunicationHandler = new FileCommunicationHandler(clientSocket);
            while (true)
            {
                ProcessMessage(fileCommunicationHandler.ReceiveMessage());
            }
        }
        private void ProcessMessage(string message)
        {
            string action = message.Substring(0, message.IndexOf(Logic.GameTransferSeparator));
            message = message.Remove(0, action.Length + Logic.GameTransferSeparator.Length);
            if (action.Equals(ETransferType.Publish.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                Sys.AddGame(game);
            }
        }
        public void ReceiveFile()
        {
            Socket client = serverSocket.Accept();
            serverSocket.Close();
            //var fileCommunication = new FileCommunicationHandler(clientSocket);
            //fileCommunication.ReceiveFile();
        }
        //public void SendMessage(string message)
        //{
        //    //_socket.Connect(_serverIpEndPoint);
        //    fileCommunicationHandler.SendMessage(message);
        //}

        //public string ReceiveMessage(Socket clientSocket)
        //{
        //_socket.Connect(_serverIpEndPoint);
        //return fileCommunicationHandler.ReceiveMessage();
        //}
    }
}