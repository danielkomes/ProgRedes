using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketSimpleClient
{
    class ClientProgram
    {
        private const string ServerIpAddress = "127.0.0.1";
        private const int ServerPort = 6000;
        private const string ClientIpAddress = "127.0.0.1";
        private const int ClientPort = 0;
        private const int FixedSize = 4;

        static void Main(string[] args)
        {
            #region Connection
            Socket clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            IPEndPoint clientEndPoint = new IPEndPoint(
                IPAddress.Parse(ClientIpAddress),
                ClientPort);
            clientSocket.Bind(clientEndPoint);
            Console.WriteLine("Trying to connect to server...");
            IPEndPoint serverEndPoint = new IPEndPoint(
                IPAddress.Parse(ServerIpAddress),
                ServerPort);
            clientSocket.Connect(serverEndPoint);
            new Thread(() => Listen(clientSocket)).Start();
            Console.WriteLine("Connected to server");
            // esto es un ejemplo para mostrar el pasaje de datos
            // por ende es válido un while (true) para hacer más sencilla la tarea
            // el while (true) nunca sería válido en un trabajo que requiera corrección
            #endregion
            while (true)
            {
                // 1 Leo el mensaje
                string message = Console.ReadLine();
                // 2 Codifico el mensaje a bytes
                byte[] data = Encoding.UTF8.GetBytes(message);
                // 3 Leo el largo de los datos codificados a bytes
                int length = data.Length;
                // 4 Codifico el largo de los datos variables a bytes
                byte[] dataLength = BitConverter.GetBytes(length);
                // 6 Envío el mensaje codificado a bytes

                SendMessage(clientSocket, dataLength);
                SendMessage(clientSocket, data);
            }
        }

        static void SendMessage(Socket clientSocket, byte[] data)
        {
            int offset = 0;
            // 5 Envío (el largo de) los datos
            try
            {
                while (offset < data.Length)
                {
                    int sent = clientSocket.Send(data, offset, data.Length - offset, SocketFlags.None);
                    if (sent == 0)
                    {
                        throw new SocketException();
                    }
                    offset += sent;
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }

        }
        static byte[] ReceiveMessage(Socket clientSocket, int bufferSize)
        {
            // 1 Leo la parte de datos que es fija
            byte[] dataLength = new byte[bufferSize];
            // 2 Recibo esos datos
            int offset = 0;
            try
            {
                while (offset < bufferSize)
                {
                    int received = clientSocket.Receive(dataLength, offset, bufferSize - offset, SocketFlags.None);
                    if (received == 0)
                    {
                        throw new SocketException();
                    }
                    offset += received;
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
            return dataLength;
        }

        static void Listen(Socket clientSocket)
        {
            while (true)
            {
                byte[] dataLength = ReceiveMessage(clientSocket, FixedSize);
                // 3 Interpreto ese valor para obtener el largo variable
                int length = BitConverter.ToInt32(dataLength);
                // 4 Creo el buffer para leer los datos
                // 5 Recibo los datos
                byte[] data = ReceiveMessage(clientSocket, length);
                // 6 Convierto (decodifico) esos datos a un string
                string message = Encoding.UTF8.GetString(data);
                // 7 Muestro los datos
                Console.WriteLine(message);
            }
        }
    }
}
