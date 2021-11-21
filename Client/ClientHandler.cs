using Common;
using Domain;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    public class ClientHandler
    {
        private readonly TcpClient tcpClient;
        private readonly IPEndPoint _clientIpEndPoint;
        private string ClientPosterFolder;
        private int ClientPort;
        private int ServerPort;
        private FileCommunicationHandler fch;

        private ClientHandler()
        {
            ReadJson();
            _clientIpEndPoint = new IPEndPoint(IPAddress.Loopback, ClientPort);
            tcpClient = new TcpClient(_clientIpEndPoint);
        }
        public static async Task<ClientHandler> ClientHandlerAsync()
        {
            ClientHandler ch = new ClientHandler();

            await ch.tcpClient.ConnectAsync(IPAddress.Loopback, ch.ServerPort);
            Console.WriteLine("Connected to server");
            ch.fch = new FileCommunicationHandler(ch.tcpClient);
            return ch;
        }
        private void ReadJson()
        {
            string filepath = "ClientConfig.json";
            using (StreamReader r = new StreamReader(filepath))
            {
                var json = r.ReadToEnd();
                var jobj = JObject.Parse(json);
                ServerPort = (int)jobj.GetValue("ServerPort");
                ClientPort = (int)jobj.GetValue("ClientPort");
                ClientPosterFolder = (string)jobj.GetValue("ClientPosterFolder");
            }
        }

        public async Task SendFileAsyncAsync(string path, string newName)
        {
            await fch.SendFileAsync(path, newName);
        }
        public async Task SendMessageAsync(ETransferType action, string message)
        {
            message = action + Logic.GameTransferSeparator + message;
            await fch.SendMessageAsync(message);
        }

        public async Task<string> ReceiveMessageAsync()
        {
            return await fch.ReceiveMessageAsync();
        }
        public async Task ReceiveFileAsync()
        {
            await fch.ReceiveFileAsync(ClientPosterFolder);
        }
        public void CloseConnection()
        {
            tcpClient.GetStream().Close();
            tcpClient.Close();
        }

    }
}