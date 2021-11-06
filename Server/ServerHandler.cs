using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using AdminServer;
using Common;
using Domain;
using Grpc.Net.Client;
using Newtonsoft.Json.Linq;
using static AdminServer.Greeter;

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
        public Dictionary<TcpClient, Client> clients;
        public ServerHandler()
        {
            ReadJson();
            _serverIpEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPort);
            tcpListener = new TcpListener(_serverIpEndPoint);
            tcpListener.Start(Backlog);

            serverRunning = true;
            clients = new Dictionary<TcpClient, Client>();
            Task.Run(async () => await GrpcSetup());
            Task.Run(async () => await AcceptClientsAsync());
        }
        private async Task GrpcSetup()
        {
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001"); //TODO: move to ServerConfig.json
            GreeterClient client = new GreeterClient(channel);
            while (true)
            {
                string input = Console.ReadLine();
                HelloReply response = await client.SayHelloAsync(
                    new HelloRequest
                    {
                        Name = input
                    });
                Console.WriteLine(response.Message);
            }
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
                    Task.Run(async () => await ListenAsync(fileCommunicationHandler, tcpClient));
                }
                catch (Exception)
                {
                    serverRunning = false;
                }
            }
        }
        async Task ListenAsync(FileCommunicationHandler fch, TcpClient tcpClient)
        {
            bool loop = true;
            while (loop && serverRunning)
            {
                try
                {
                    string msg = await fch.ReceiveMessageAsync();
                    loop = await ProcessMessageAsync(tcpClient, fch, msg);
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
        private async Task<bool> ProcessMessageAsync(TcpClient tcpClient, FileCommunicationHandler fch, string message)
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
                        //await SendMessageAsync(fch, Logic.EncodeOwnedGames(client.OwnedGames));
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
                List<Game> list = Sys.GetGames();
                await SendMessageAsync(fch, Logic.EncodeListGames(list));
            }
            else if (action.Equals(ETransferType.Owned.ToString()))
            {
                Client c = Sys.GetClient(message);
                await SendMessageAsync(fch, Logic.EncodeOwnedGames(c.OwnedGames));
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
                //if (response)
                //{
                //    await SendMessageAsync(fch, Logic.EncodeOwnedGames(Sys.GetClient(username).OwnedGames));
                //}
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