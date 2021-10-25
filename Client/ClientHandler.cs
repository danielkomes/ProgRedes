using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common;
using Domain;
using Newtonsoft.Json.Linq;

namespace Client
{
    public class ClientHandler
    {
        private readonly TcpClient tcpClient;
        private readonly IPEndPoint _clientIpEndPoint;
        private string ClientPosterFolder;
        private int ClientPort;
        private int ServerPort;
        private readonly FileCommunicationHandler fch;

        public ClientHandler()
        {
            ReadJson();
            _clientIpEndPoint = new IPEndPoint(IPAddress.Loopback, ClientPort);
            tcpClient = new TcpClient(_clientIpEndPoint);
            await ConnectAsync();
            Console.WriteLine("Connected to server");
            fch = new FileCommunicationHandler(tcpClient);
        }
        private async Task ConnectAsync()
        {
            await tcpClient.ConnectAsync(IPAddress.Loopback, ServerPort);
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

        public async Task SendFileAsync(string path, string newName)
        {
            await fch.SendFileAsync(path, newName);
        }
        public async Task SendMessage(ETransferType action, string message)
        {
            message = action + Logic.GameTransferSeparator + message;
            await fch.SendMessageAsync(message);
        }

        public async Task<string> ReceiveMessage()
        {
            return await fch.ReceiveMessageAsync();
        }
        public async Task ReceiveFile()
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