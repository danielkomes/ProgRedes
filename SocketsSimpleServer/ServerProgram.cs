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

        private static List<ClientThread> clientThreads;
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
            clientThreads = new List<ClientThread>();
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
                clientThreads.Add(new ClientThread(clientSocket));
                Console.WriteLine("new client connected");
                new Thread(() => Listen(clientSocket)).Start();
                new Thread(() => ResponseToClient(clientSocket)).Start();
            }
        }
        static void ResponseToClient(Socket clientSocket)
        {
            ClientThread ct = clientThreads.Find(t => t.Socket.Equals(clientSocket));
            ct.LocationRequest = "0";
            while (true)
            {
                if (!ct.LocationRequest.Equals(""))
                {
                    //console.Response(ct);
                    string message = ct.OptionsResponse;
                    // 1 Leo el mensaje
                    // 2 Codifico el mensaje a bytes
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    // 3 Leo el largo de los datos codificados a bytes
                    int length = data.Length;
                    // 4 Codifico el largo de los datos variables a bytes
                    byte[] dataLength = BitConverter.GetBytes(length);
                    SendMessage(clientSocket, dataLength);
                    SendMessage(clientSocket, data);
                }
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
            byte[] data = new byte[bufferSize];
            // 2 Recibo esos datos
            int offset = 0;
            try
            {
                while (offset < bufferSize)
                {
                    int received = clientSocket.Receive(data, offset, bufferSize - offset, SocketFlags.None);
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
            return data;
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
                ClientThread ct = clientThreads.Find(t => t.Socket.Equals(clientSocket));
                ct.LocationRequest = message;
                // 7 Muestro los datos
                //Console.WriteLine(message); ahora hay que responder con las opciones de consola. ResponseToClients() lo hará automáticamente si corresponde

            }
        }
    }
}
