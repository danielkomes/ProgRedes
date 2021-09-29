using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common;
using Domain;
using Newtonsoft.Json.Linq;

namespace Client
{
    public class ClientHandler
    {
        private readonly Socket clientSocket;
        private readonly IPEndPoint _clientIpEndPoint;
        private readonly IPEndPoint _serverIpEndPoint;
        private string ClientPosterFolder;
        private int ClientPort;
        private int ServerPort;
        private FileCommunicationHandler fch;

        public ClientHandler()
        {
            ReadJson();
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientIpEndPoint = new IPEndPoint(IPAddress.Loopback, ClientPort);
            _serverIpEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPort);
            clientSocket.Bind(_clientIpEndPoint);
            clientSocket.Connect(_serverIpEndPoint);
            Console.WriteLine("Connected to server");
            fch = new FileCommunicationHandler(clientSocket);
        }
        private void ReadJson()
        {
            string filepath = "../../../ClientConfig.json";
            using (StreamReader r = new StreamReader(filepath))
            {
                var json = r.ReadToEnd();
                var jobj = JObject.Parse(json);
                ServerPort = (int)jobj.GetValue("ServerPort");
                ClientPort = (int)jobj.GetValue("ClientPort");
                ClientPosterFolder = (string)jobj.GetValue("ClientPosterFolder");
            }
        }

        public void SendFile(string path, string newName)
        {
            fch.SendFile(path, newName);
        }
        public void SendMessage(ETransferType action, string message)
        {
            message = action + Logic.GameTransferSeparator + message;
            fch.SendMessage(message);
        }

        public string ReceiveMessage()
        {
            //_socket.Connect(_serverIpEndPoint);
            return fch.ReceiveMessage();
        }
        public void ReceiveFile()
        {
            fch.ReceiveFile(ClientPosterFolder);
        }
        public void CloseConnection()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

    }
}