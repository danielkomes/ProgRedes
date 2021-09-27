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
        }
        public void ReceiveFile()
        {
            Socket client = serverSocket.Accept();
            serverSocket.Close();
            //var fileCommunication = new FileCommunicationHandler(clientSocket);
            //fileCommunication.ReceiveFile();
        }
        public void SendMessage(FileCommunicationHandler fch, string message)
        {
            //_socket.Connect(_serverIpEndPoint);
            fch.SendMessage(message);
        }

        //public string ReceiveMessage(Socket clientSocket)
        //{
        //_socket.Connect(_serverIpEndPoint);
        //return fileCommunicationHandler.ReceiveMessage();
        //}
    }
}