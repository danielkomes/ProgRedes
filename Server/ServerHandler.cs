using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Domain;
using Newtonsoft.Json.Linq;

namespace Server
{
    public class ServerHandler
    {
        private readonly TcpListener tcpListener;
        private readonly IPEndPoint _serverIpEndPoint;

        private string ServerPosterFolder;
        private int ServerPort;
        private int Backlog;
        private bool serverRunning;
        private Thread acceptClients;
        public Dictionary<TcpClient, Client> clients;
        public ServerHandler()
        {
            ReadJson();
            _serverIpEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPort);
            tcpListener = new TcpListener(_serverIpEndPoint);
            tcpListener.Start(Backlog);

            serverRunning = true;
            clients = new Dictionary<TcpClient, Client>();
            acceptClients = new Thread(async () => await AcceptClientsAsync());
            acceptClients.Start();
        }

        private void ReadJson()
        {
            string filepath = "ServerConfig.json";
            using (StreamReader r = new StreamReader(filepath))
            {
                var json = r.ReadToEnd();
                var jobj = JObject.Parse(json);
                ServerPort = (int)jobj.GetValue("ServerPort");
                Backlog = (int)jobj.GetValue("Backlog");
                ServerPosterFolder = (string)jobj.GetValue("ServerPosterFolder");
            }
        }
        private async Task AcceptClientsAsync()
        {
            while (serverRunning)
            {
                try //para interrumpir el Accept()
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    clients.Add(tcpClient, null);
                    Console.WriteLine("New client connected. Total: " + clients.Count);
                    FileCommunicationHandler fileCommunicationHandler = new FileCommunicationHandler(tcpClient);
                    new Thread(async () => await Listen(fileCommunicationHandler, tcpClient)).Start();
                }
                catch (Exception)
                {
                    serverRunning = false;
                }
            }
        }
        async Task Listen(FileCommunicationHandler fch, TcpClient tcpClient)
        {
            bool loop = true;
            while (loop && serverRunning)
            {
                try
                {
                    loop = await ProcessMessage(tcpClient, fch, fch.ReceiveMessageAsync().Result);
                }
                catch (Exception)
                {
                    //tcpClient.GetStream().Close();
                    clients.Remove(tcpClient);
                    Console.WriteLine("Client disconnected. Total: " + clients.Count);
                    loop = false;
                }
            }
        }
        private async Task<bool> ProcessMessage(TcpClient tcpClient, FileCommunicationHandler fch, string message)
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
                        clients[tcpClient] = client;
                        await SendMessageAsync(fch, "true");
                        await SendMessageAsync(fch, Logic.EncodeOwnedGames(client.OwnedGames));
                    }
                    else
                    {
                        await SendMessageAsync(fch, "false");
                    }
                }
                else
                {
                    await SendMessageAsync(fch, "false");
                }
            }
            else if (action.Equals(ETransferType.Signup.ToString()))
            {
                bool msg = Sys.AddClient(message);
                if (msg)
                {
                    Client client = Sys.GetClient(message);
                    client.IsOnline = true;
                    clients[tcpClient] = client;
                }
                await SendMessageAsync(fch, msg + "");
            }
            else if (action.Equals(ETransferType.Logoff.ToString()))
            {
                Sys.GetClient(message).IsOnline = false;
            }
            else if (action.Equals(ETransferType.Publish.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                Sys.AddGame(game);
                await ReceiveFileAsync(fch, game.Id + ".jpg");
            }
            else if (action.Equals(ETransferType.List.ToString()))
            {
                await SendMessageAsync(fch, Logic.EncodeListGames(Sys.Games));
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
                await SendFile(fch, ServerPosterFolder + id + ".jpg", game.Title + ".jpg");
            }
            else if (action.Equals(ETransferType.BuyGame.ToString()))
            {
                string[] arr = message.Split(Logic.GameTransferSeparator);
                int gameId = int.Parse(arr[0]);
                string username = arr[1];
                bool response = Sys.BuyGame(username, gameId);
                await SendMessageAsync(fch, response + "");
                if (response)
                {
                    await SendMessageAsync(fch, Logic.EncodeOwnedGames(Sys.GetClient(username).OwnedGames));
                }
            }
            else if (action.Equals(ETransferType.Disconnect.ToString()))
            {
                tcpClient.GetStream().Close();
                clients.Remove(tcpClient);
                Console.WriteLine("Client disconnected. Total: " + clients.Count);
                ret = false;
            }
            return ret;
        }
        public async Task ReceiveFileAsync(FileCommunicationHandler fch, string newName)
        {
            await fch.ReceiveFileAsync(ServerPosterFolder, newName);
        }
        public async Task SendFile(FileCommunicationHandler fch, string path, string newName)
        {
            await fch.SendFileAsync(path, newName);
        }
        public async Task SendMessageAsync(FileCommunicationHandler fch, string message)
        {
            await fch.SendMessageAsync(message);
        }
        public void KickClient(Client client)
        {
            foreach (var c in clients)
            {
                if (c.Value.Username.Equals(client.Username))
                {
                    c.Key.GetStream().Close();
                    //clients.Remove(c.Key);
                    //Console.WriteLine("Client disconnected. Total: " + clients.Count);
                }
            }
        }
        public void CloseConnection()
        {
            tcpListener.Stop();
            foreach (var clients in clients)
            {
                clients.Key.GetStream().Close();
            }
            serverRunning = false;
        }
    }
}