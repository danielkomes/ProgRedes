using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Domain;

namespace SocketsSimpleServer
{
    class ServerProgram
    {
        private static Socket serverSocket;
        private const string ServerIpAddress = "127.0.0.1";
        private const int ServerPort = 6000;
        private const int Backlog = 4;
        private const int FixedSize = 4;
        private static List<Socket> clients;
        //private static ClientConsoleOld console;

        static void Main(string[] args)
        {
            #region Connection
            serverSocket = new Socket(
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
            clients = new List<Socket>();
            //console = new ClientConsoleOld();
            Thread AcceptClientsThread = new Thread(() => AcceptClients());
            AcceptClientsThread.Start();
            //serverSocket.Close();
            // De momento seguimos dejando un while true sin condición de parada
            // a efectos de hacer una demo
            #endregion

        }
        static void AcceptClients()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                clients.Add(clientSocket);
                Console.WriteLine("New client connected. Total: " + clients.Count);
                new Thread(() => Listen(clientSocket)).Start();
                new Thread(() => ResponseToClient(clientSocket)).Start();
            }
        }
        static void ResponseToClient(Socket clientSocket)
        {
            while (true)
            {
                // 1 Leo el mensaje
                string message = Console.ReadLine();
                SendMessage(clientSocket, message);
            }
        }

        public static void SendMessage(Socket clientSocket, string message)
        {
            // 2 Codifico el mensaje a bytes
            byte[] data = Encoding.UTF8.GetBytes(message);
            // 3 Leo el largo de los datos codificados a bytes
            int length = data.Length;
            // 4 Codifico el largo de los datos variables a bytes
            byte[] dataLength = BitConverter.GetBytes(length);
            // 6 Envío el mensaje codificado a bytes

            SendMessageAux(clientSocket, dataLength);
            SendMessageAux(clientSocket, data);
        }
        private static void SendMessageAux(Socket clientSocket, byte[] data)
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
        public static string ReceiveMessage(Socket clientSocket)
        {
            byte[] dataLength = ReceiveMessageAux(clientSocket, FixedSize);
            // 3 Interpreto ese valor para obtener el largo variable
            int length = BitConverter.ToInt32(dataLength);
            // 4 Creo el buffer para leer los datos
            // 5 Recibo los datos
            byte[] data = ReceiveMessageAux(clientSocket, length);
            // 6 Convierto (decodifico) esos datos a un string
            string message = Encoding.UTF8.GetString(data);
            // 7 Muestro los datos
            return message;
        }
        static byte[] ReceiveMessageAux(Socket clientSocket, int bufferSize)
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
                Console.WriteLine(ReceiveMessage(clientSocket));
            }
        }
    }
}
