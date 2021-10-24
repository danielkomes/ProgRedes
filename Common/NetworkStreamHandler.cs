using System.Net.Sockets;

namespace Common
{
    public class NetworkStreamHandler
    {
        private readonly NetworkStream networkStream;

        public NetworkStreamHandler(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
        }

        public byte[] ReadData(int length)
        {
            int offset = 0;
            byte[] response = new byte[length];
            while (offset < length)
            {
                int received = networkStream.Read(response, offset, length - offset);
                if (received == 0)
                {
                    throw new SocketException();
                }

                offset += received;
            }
            return response;
        }

        public void SendData(byte[] data)
        {
            networkStream.Write(data, 0, data.Length);
        }
    }
}