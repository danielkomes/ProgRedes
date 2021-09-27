using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common;
using Domain;

namespace Client
{
    public class ClientHandler
    {
        private readonly Socket clientSocket;
        private readonly IPEndPoint _clientIpEndPoint;
        private readonly IPEndPoint _serverIpEndPoint;
        private const string ClientPosterFolder = "Posters/";
        private FileCommunicationHandler fch;

        public ClientHandler()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientIpEndPoint = new IPEndPoint(IPAddress.Loopback, 0);
            _serverIpEndPoint = new IPEndPoint(IPAddress.Loopback, 7000);
            clientSocket.Bind(_clientIpEndPoint);
            clientSocket.Connect(_serverIpEndPoint);
            fch = new FileCommunicationHandler(clientSocket);
            new Thread(() => Listen()).Start();
        }
        private void Listen()
        {
            while (true)
            {
                //ReceiveMessage();
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