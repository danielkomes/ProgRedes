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
//using static AdminServer.Greeter;
using static AdminServer.MessageExchanger;

namespace Server
{
    public class ServerHandler
    {
        private readonly TcpListener tcpListener;
        private readonly IPEndPoint _serverIpEndPoint;
        MessageExchangerClient client;

        private string ServerPosterFolder;
        private int ServerPort;
        private int Backlog;
        private bool serverRunning;
        public Dictionary<TcpClient, string> clients;
        public ServerHandler()
        {
            ReadJson();
            _serverIpEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPort);
            tcpListener = new TcpListener(_serverIpEndPoint);
            tcpListener.Start(Backlog);

            serverRunning = true;
            clients = new Dictionary<TcpClient, string>();
            GrpcSetup();
            Task.Run(async () => await AcceptClientsAsync());
        }
        private void GrpcSetup()
        {
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001"); //TODO: move to ServerConfig.json
            client = new MessageExchangerClient(channel);
            //while (true)
            //{
            //    string input = Console.ReadLine();
            //    HelloReply response = await client.SayHelloAsync(
            //        new HelloRequest
            //        {
            //            Name = input
            //        });
            //    Console.WriteLine(response.Message);
            //}
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
                    MessageReply reply = await client.LogoffAsync(
                        new MessageRequest
                        {
                            Message = clients[tcpClient]
                        });
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
                MessageReply reply = await LoginAsync(message);
                string RpcReply = reply.Message;
                if (bool.Parse(RpcReply))
                {
                    clients[tcpClient] = message;
                }
                await SendMessageAsync(fch, RpcReply);
            }
            else if (action.Equals(ETransferType.Signup.ToString()))
            {
                MessageReply reply = await SignupAsync(message);
                string RpcReply = reply.Message;
                if (bool.Parse(RpcReply))
                {
                    clients[tcpClient] = message;
                }
                await SendMessageAsync(fch, RpcReply);
            }
            else if (action.Equals(ETransferType.Logoff.ToString()))
            {
                MessageReply reply = await LogoffAsync(message);
                string RpcReply = reply.Message;
            }
            else if (action.Equals(ETransferType.Publish.ToString()))
            {
                PublishReply reply = await PublishAsync(message);
                string gameId = reply.Id;
                string gameTitle = reply.Title;

                await ReceiveFileAsync(fch, gameId + ".jpg");
                //TODO: change old file transference methods to not create files under Server (must be created under AdminServer)

                byte[] fileData = await File.ReadAllBytesAsync(ServerPosterFolder + gameId + ".jpg");
                MessageReply fileReply = await client.ReceiveFileAsync(
                    new FileExchange
                    {
                        FileName = gameId,
                        FileData = Google.Protobuf.ByteString.CopyFrom(fileData)
                    });
                string RpcFileReply = fileReply.Message;
            }
            else if (action.Equals(ETransferType.List.ToString()))
            {
                MessageReply reply = await ListAsync(message);
                string RpcReply = reply.Message;
                //    List<Game> list = Sys.GetGames();
                await SendMessageAsync(fch, RpcReply);
            }
            else if (action.Equals(ETransferType.Owned.ToString()))
            {
                MessageReply reply = await OwnedAsync(message);
                string RpcReply = reply.Message;
                await SendMessageAsync(fch, RpcReply);
            }
            else if (action.Equals(ETransferType.Edit.ToString()))
            {
                MessageReply reply = await EditAsync(message);
                string RpcReply = reply.Message;
            }
            else if (action.Equals(ETransferType.Delete.ToString()))
            {
                MessageReply reply = await DeleteAsync(message);
                string RpcReply = reply.Message;
            }
            else if (action.Equals(ETransferType.Review.ToString()))
            {
                MessageReply reply = await ReviewAsync(message);
                string RpcReply = reply.Message;
            }
            else if (action.Equals(ETransferType.Download.ToString()))
            {
                FileExchange reply = await DownloadAsync(message);
                string fileId = reply.FileId;
                string fileName = reply.FileName;
                byte[] fileData = reply.FileData.ToByteArray();
                //TODO: change old file transference methods to not create files under Server (must be created under AdminServer)
                await SendFile(fch, ServerPosterFolder + fileId + ".jpg", fileName + ".jpg");
            }
            else if (action.Equals(ETransferType.BuyGame.ToString()))
            {
                MessageReply reply = await BuyGameAsync(message);
                string RpcReply = reply.Message;
                await SendMessageAsync(fch, RpcReply);
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

        public async Task<MessageReply> LoginAsync(string message)
        {
            MessageReply reply = await client.LoginAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<MessageReply> SignupAsync(string message)
        {
            MessageReply reply = await client.SignupAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<MessageReply> LogoffAsync(string message)
        {
            MessageReply reply = await client.LogoffAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<PublishReply> PublishAsync(string message)
        {
            PublishReply reply = await client.PublishAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<MessageReply> ListAsync(string message)
        {
            MessageReply reply = await client.ListAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<MessageReply> OwnedAsync(string message)
        {
            MessageReply reply = await client.OwnedAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<MessageReply> EditAsync(string message)
        {
            MessageReply reply = await client.EditAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<MessageReply> DeleteAsync(string message)
        {
            MessageReply reply = await client.DeleteAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<MessageReply> ReviewAsync(string message)
        {
            MessageReply reply = await client.ReviewAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<FileExchange> DownloadAsync(string message)
        {
            FileExchange reply = await client.DownloadAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<MessageReply> BuyGameAsync(string message)
        {
            MessageReply reply = await client.BuyGameAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }

        public async Task<MessageReply> ListClientsAsync(string message)
        {
            MessageReply reply = await client.ListClientsAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<MessageReply> RemoveGameFromClientAsync(string username, int gameId)
        {
            MessageReply reply = await client.RemoveGameFromClientAsync(
                new RemoveGameFromClientRequest
                {
                    Username = username,
                    GameId = gameId
                });
            return reply;
        }
        public async Task<MessageReply> RemoveAllGamesFromClientAsync(string message)
        {
            MessageReply reply = await client.RemoveAllGamesFromClientAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
        }
        public async Task<MessageReply> DeleteClientAsync(string message)
        {
            MessageReply reply = await client.DeleteClientAsync(
                new MessageRequest
                {
                    Message = message
                });
            return reply;
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
        public void KickClient(string client)
        {
            foreach (var c in clients)
            {
                if (c.Value.Equals(client))
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