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
        private const string ServerPosterFolder = "Posters/";
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
                FileCommunicationHandler fileCommunicationHandler = new FileCommunicationHandler(clientSocket);
                new Thread(() => Listen(fileCommunicationHandler)).Start();
            }
        }
        void Listen(FileCommunicationHandler fch)
        {
            while (true)
            {
                ProcessMessage(fch, fch.ReceiveMessage());
            }
        }
        private void ProcessMessage(FileCommunicationHandler fch, string message)
        {
            string action = message.Substring(0, message.IndexOf(Logic.GameTransferSeparator));
            message = message.Remove(0, action.Length + Logic.GameTransferSeparator.Length);
            if (action.Equals(ETransferType.Publish.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                Sys.AddGame(game);
                ReceiveFile(fch);
            }
            else if (action.Equals(ETransferType.List.ToString()))
            {
                SendMessage(fch, Logic.EncodeListGames(Sys.Games));
            }
            else if (action.Equals(ETransferType.Edit.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                Sys.ReplaceGame(game);
            }
            else if (action.Equals(ETransferType.Delete.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                Sys.DeleteGame(game);
            }
            else if (action.Equals(ETransferType.Review.ToString()))
            {
                string[] arr = message.Split(Logic.GameTransferSeparator);
                int id = int.Parse(arr[0]);
                Review r = Logic.DecodeReview(arr[1]);
                Sys.AddReview(id, r);
            }
            else if (action.Equals(ETransferType.Download.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                int id = game.Id;
                SendFile(fch, ServerPosterFolder + id + ".jpg", game.Title + ".jpg");
            }
        }
        public void ReceiveFile(FileCommunicationHandler fch)
        {
            fch.ReceiveFile(ServerPosterFolder);
        }
        public void SendFile(FileCommunicationHandler fch, string path, string newName)
        {
            fch.SendFile(path, newName);
        }
        public void SendMessage(FileCommunicationHandler fch, string message)
        {
            //_socket.Connect(_serverIpEndPoint);
            fch.SendMessage(message);
        }
    }
}