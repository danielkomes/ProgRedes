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
        private bool socketOpen;
        private Thread acceptClients;
        public List<Socket> clients;
        public ServerHandler()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverIpEndPoint = new IPEndPoint(IPAddress.Loopback, 7000);
            serverSocket.Bind(_serverIpEndPoint);
            serverSocket.Listen(1);
            socketOpen = true;
            clients = new List<Socket>();
            acceptClients = new Thread(() => AcceptClients());
            acceptClients.Start();
        }
        void AcceptClients()
        {
            while (socketOpen)
            {
                try
                {
                    Socket clientSocket = serverSocket.Accept();
                    clients.Add(clientSocket);
                    Console.WriteLine("New client connected. Total: " + clients.Count);
                    FileCommunicationHandler fileCommunicationHandler = new FileCommunicationHandler(clientSocket);
                    new Thread(() => Listen(fileCommunicationHandler)).Start();
                }
                catch (SocketException)
                {
                    socketOpen = false;
                }
            }
        }
        void Listen(FileCommunicationHandler fch)
        {
            bool loop = true;
            while (loop && socketOpen)
            {
                loop = ProcessMessage(fch, fch.ReceiveMessage());
            }
        }
        private bool ProcessMessage(FileCommunicationHandler fch, string message)
        {
            string action = message.Substring(0, message.IndexOf(Logic.GameTransferSeparator));
            message = message.Remove(0, action.Length + Logic.GameTransferSeparator.Length);
            bool ret = true;
            if (action.Equals(ETransferType.Login.ToString()))
            {
                Client client = Sys.GetClient(message);
                if (client != null)
                {
                    if (!client.IsOnline)
                    {
                        client.IsOnline = true;
                        SendMessage(fch, "true");
                        SendMessage(fch, Logic.EncodeOwnedGames(client.OwnedGames));
                    }
                    else
                    {
                        SendMessage(fch, "false");
                    }
                }
                else
                {
                    SendMessage(fch, "false");
                }
            }
            else if (action.Equals(ETransferType.Signup.ToString()))
            {
                bool msg = Sys.AddClient(message);
                if (msg)
                {
                    Sys.GetClient(message).IsOnline = true;
                }
                SendMessage(fch, msg + "");
            }
            else if (action.Equals(ETransferType.Logoff.ToString()))
            {
                Sys.GetClient(message).IsOnline = false;
            }
            else if (action.Equals(ETransferType.Publish.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                Sys.AddGame(game);
                ReceiveFile(fch, game.Id + ".jpg");
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
            else if (action.Equals(ETransferType.BuyGame.ToString()))
            {
                string[] arr = message.Split(Logic.GameTransferSeparator);
                int gameId = int.Parse(arr[0]);
                string username = arr[1];
                bool response = Sys.BuyGame(username, gameId);
                SendMessage(fch, response + "");
                if (response)
                {
                    SendMessage(fch, Logic.EncodeOwnedGames(Sys.GetClient(username).OwnedGames));
                }
            }
            else if (action.Equals(ETransferType.Disconnect.ToString()))
            {
                ret = false;
                Console.WriteLine("Client disconnected. Total: " + clients.Count);
            }
            return ret;
        }
        public void ReceiveFile(FileCommunicationHandler fch, string newName)
        {
            fch.ReceiveFile(ServerPosterFolder, newName);
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
        public void CloseConnection()
        {
            socketOpen = false;
            //acceptClients.Suspend();
            //acceptClients.Abort();
                serverSocket.Close();
                serverSocket.Shutdown(SocketShutdown.Both);
        }
    }
}