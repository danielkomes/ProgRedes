using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class FileCommunicationHandler
    {
        private readonly FileStreamHandler _fileStreamHandler;
        private readonly NetworkStreamHandler _socketStreamHandler;

        public static void Main(string[] args)
        {

        }

        public FileCommunicationHandler(TcpClient tcpClient)
        {
            _socketStreamHandler = new NetworkStreamHandler(tcpClient.GetStream());
            _fileStreamHandler = new FileStreamHandler();
        }

        public void SendFile(string path, string newName)
        {
            var fileInfo = new FileInfo(path);
            byte[] fileNameData = Encoding.UTF8.GetBytes(newName);
            int fileNameLength = fileNameData.Length;
            byte[] fileNameLengthData = BitConverter.GetBytes(fileNameLength);
            _socketStreamHandler.SendData(fileNameLengthData);
            _socketStreamHandler.SendData(fileNameData);

            long fileSize = fileInfo.Length;
            byte[] fileSizeDataLength = BitConverter.GetBytes(fileSize);
            _socketStreamHandler.SendData(fileSizeDataLength);
            SendFile(fileSize, path);
        }

        public void ReceiveFile(string pathNoName, string newName = "")
        {
            byte[] fileNameLengthData = _socketStreamHandler.ReadData(ProtocolSpecification.FileNameSize);
            int fileNameLength = BitConverter.ToInt32(fileNameLengthData);
            byte[] fileNameData = _socketStreamHandler.ReadData(fileNameLength);
            string fileName = Encoding.UTF8.GetString(fileNameData);
            if (!string.IsNullOrEmpty(newName))
            {
                fileName = newName;
            }
            byte[] fileSizeDataLength = _socketStreamHandler.ReadData(ProtocolSpecification.FileSize);
            long fileSize = BitConverter.ToInt64(fileSizeDataLength);
            ReceiveFile(fileSize, pathNoName + fileName);
        }

        private void SendFile(long fileSize, string path)
        {
            long fileParts = ProtocolSpecification.CalculateParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart != fileParts)
                {
                    data = _fileStreamHandler.ReadData(path, ProtocolSpecification.MaxPacketSize, offset);
                    offset += ProtocolSpecification.MaxPacketSize;
                }
                else
                {
                    int lastPartSize = (int)(fileSize - offset);
                    data = _fileStreamHandler.ReadData(path, lastPartSize, offset);
                    offset += lastPartSize;
                }

                _socketStreamHandler.SendData(data);
                currentPart++;
            }
        }

        private void ReceiveFile(long fileSize, string fileName)
        {
            long fileParts = ProtocolSpecification.CalculateParts(fileSize);
            long offset = 0;
            long currentPart = 1;
            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart != fileParts)
                {
                    data = _socketStreamHandler.ReadData(ProtocolSpecification.MaxPacketSize);
                    offset += ProtocolSpecification.MaxPacketSize;
                }
                else
                {
                    int lastPartSize = (int)(fileSize - offset);
                    data = _socketStreamHandler.ReadData(lastPartSize);
                    offset += lastPartSize;
                }
                _fileStreamHandler.WriteData(fileName, data);
                currentPart++;
            }
        }
        public string ReceiveMessage()
        {
            byte[] dataLength = _socketStreamHandler.ReadData(ProtocolSpecification.FileNameSize);
            int length = BitConverter.ToInt32(dataLength);
            byte[] data = _socketStreamHandler.ReadData(length);
            string message = Encoding.UTF8.GetString(data);
            return message;
        }
        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            int length = data.Length;
            byte[] dataLength = BitConverter.GetBytes(length);

            _socketStreamHandler.SendData(dataLength);
            _socketStreamHandler.SendData(data);
        }
    }
}