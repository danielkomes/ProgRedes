using System.Net;
using System.Net.Sockets;
using Common;
using Domain;

namespace Client
{
    public class ClientHandler
    {
        private readonly Socket _socket;
        private readonly IPEndPoint _clientIpEndPoint;
        private readonly IPEndPoint _serverIpEndPoint;

        public ClientHandler()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientIpEndPoint = new IPEndPoint(IPAddress.Loopback, 0);
            _serverIpEndPoint = new IPEndPoint(IPAddress.Loopback, 7000);
            _socket.Bind(_clientIpEndPoint);
            _socket.Connect(_serverIpEndPoint);
        }

        public void SendFile(string path)
        {
            //_socket.Connect(_serverIpEndPoint);
            var fileCommunication = new FileCommunicationHandler(_socket);
            fileCommunication.SendFile(path);
        }
        public void SendMessage(ETransferType action, string message)
        {
            //_socket.Connect(_serverIpEndPoint);
            FileCommunicationHandler fch = new FileCommunicationHandler(_socket);
            message = action + Logic.GameTransferSeparator + message;
            fch.SendMessage(message);
        }

        public string ReceiveMessage()
        {
            //_socket.Connect(_serverIpEndPoint);
            FileCommunicationHandler fch = new FileCommunicationHandler(_socket);
            return fch.ReceiveMessage();
        }
        public void CloseConnection()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

    }
}