using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketsSimpleServer
{
    class ServerProgram
    {
        private const string ServerIpAddress = "127.0.0.1";
        private const int ServerPort = 6000;
        private const int Backlog = 4;
        private const int FixedSize = 4;

        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            IPEndPoint serverIpEndPoint = new IPEndPoint(
                IPAddress.Parse(ServerIpAddress),
                ServerPort);
            // Asocio el socket a un par ip / puerto (endpoint)
            serverSocket.Bind(serverIpEndPoint);
            // Dejo al socket en estado de escucha y escucho conexiones
            // el backlog es la cantidad de clientes que podemos tener sin atender
            // conectados al servidor
            serverSocket.Listen(Backlog);
            Console.WriteLine("Start listening for client");
            // Capturo al primer cliente que se quiera conectar
            Socket clientSocket = serverSocket.Accept();
            serverSocket.Close();
            new Thread(() => Listen(clientSocket)).Start();
            // De momento seguimos dejando un while true sin condición de parada
            // a efectos de hacer una demo
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
                int offset = 0;
                // 5 Envío el largo de los datos
                try
                {
                    while (offset < dataLength.Length)
                    {
                        int sent = clientSocket.Send(dataLength, offset, dataLength.Length - offset, SocketFlags.None);
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
                // 6 Envío el mensaje codificado a bytes
                offset = 0;
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
        }

        static void Listen(Socket clientSocket)
        {
            while (true)
            {
                // 1 Leo la parte de datos que es fija
                byte[] dataLength = new byte[FixedSize];
                // 2 Recibo esos datos
                int offset = 0;
                try
                {
                    while (offset < FixedSize)
                    {
                        int received = clientSocket.Receive(dataLength, offset, FixedSize - offset, SocketFlags.None);
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
                // 3 Interpreto ese valor para obtener el largo variable
                int length = BitConverter.ToInt32(dataLength);
                // 4 Creo el buffer para leer los datos
                byte[] data = new byte[length];
                // 5 Recibo los datos
                offset = 0;
                try
                {
                    while (offset < length)
                    {
                        int received = clientSocket.Receive(data, offset, length - offset, SocketFlags.None);
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
                // 6 Convierto (decodifico) esos datos a un string
                string message = Encoding.UTF8.GetString(data);
                // 7 Muestro los datos
                Console.WriteLine(message + "mem dir: " + clientSocket.GetHashCode());
            }
        }
    }
}
