using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common;
using Domain;
using Newtonsoft.Json.Linq;

namespace Server
{
    public class ServerHandler
    {
        private readonly Socket serverSocket;
        private readonly IPEndPoint _serverIpEndPoint;
        private string ServerPosterFolder;
        private int ServerPort;
        private int Backlog;
        private bool socketOpen;
        private Thread acceptClients;
        public List<Socket> clients;
        public ServerHandler()
        {
            ReadJson();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverIpEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPort);
            serverSocket.Bind(_serverIpEndPoint);
            serverSocket.Listen(Backlog);
            socketOpen = true;
            clients = new List<Socket>();
            acceptClients = new Thread(() => AcceptClients());
            acceptClients.Start();
        }

        private void ReadJson()
        {
            string filepath = "../../../ServerConfig.json";
            using (StreamReader r = new StreamReader(filepath))
            {
                var json = r.ReadToEnd();
                var jobj = JObject.Parse(json);
                ServerPort = (int)jobj.GetValue("ServerPort");
                Backlog = (int)jobj.GetValue("Backlog");
                ServerPosterFolder = (string)jobj.GetValue("ServerPosterFolder");
            }
        }
        void AcceptClients()
        {
            while (socketOpen)
            {
                try //para interrumpir el Accept()
                {
                    Socket clientSocket = serverSocket.Accept();
                    clients.Add(clientSocket);
                    Console.WriteLine("New client connected. Total: " + clients.Count);
                    FileCommunicationHandler fileCommunicationHandler = new FileCommunicationHandler(clientSocket);
                    new Thread(() => Listen(fileCommunicationHandler, clientSocket)).Start();
                }
                catch (SocketException)
                {
                    socketOpen = false;
                }
            }
        }
        void Listen(FileCommunicationHandler fch, Socket clientSocket)
        {
            bool loop = true;
            while (loop && socketOpen)
            {
                try
                {
                    loop = ProcessMessage(fch, fch.ReceiveMessage());
                }
                catch (SocketException)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clients.Remove(clientSocket);
                    Console.WriteLine("Client disconnected. Total: " + clients.Count);
                    loop = false;
                }
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
            fch.SendMessage(message);
        }
        public void CloseConnection()
        {
            serverSocket.Close();
            foreach (Socket s in clients)
            {
                s.Shutdown(SocketShutdown.Both);
            }
            socketOpen = false;
        }
    }
}