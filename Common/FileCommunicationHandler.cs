using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FileCommunicationHandler
    {
        private readonly FileStreamHandler _fileStreamHandler;
        private readonly NetworkStreamHandler networkStreamHandler;

        public static void Main(string[] args)
        {

        }

        public FileCommunicationHandler(TcpClient tcpClient)
        {
            networkStreamHandler = new NetworkStreamHandler(tcpClient.GetStream());
            _fileStreamHandler = new FileStreamHandler();
        }

        public async Task SendFileAsync(string path, string newName)
        {
            var fileInfo = new FileInfo(path);
            byte[] fileNameData = Encoding.UTF8.GetBytes(newName);
            int fileNameLength = fileNameData.Length;
            byte[] fileNameLengthData = BitConverter.GetBytes(fileNameLength);
            await networkStreamHandler.SendDataAsync(fileNameLengthData);
            await networkStreamHandler.SendDataAsync(fileNameData);

            long fileSize = fileInfo.Length;
            byte[] fileSizeDataLength = BitConverter.GetBytes(fileSize);
            await networkStreamHandler.SendDataAsync(fileSizeDataLength);
            await SendFileAsync(fileSize, path);
        }
        public async Task SendFileAsync(byte[] data, string newName)
        {
            byte[] fileNameData = Encoding.UTF8.GetBytes(newName);
            int fileNameLength = fileNameData.Length;
            byte[] fileNameLengthData = BitConverter.GetBytes(fileNameLength);
            await networkStreamHandler.SendDataAsync(fileNameLengthData);
            await networkStreamHandler.SendDataAsync(fileNameData);

            long fileSize = data.Length;
            byte[] fileSizeDataLength = BitConverter.GetBytes(fileSize);
            await networkStreamHandler.SendDataAsync(fileSizeDataLength);
            await networkStreamHandler.SendDataAsync(data);
        }


        public async Task ReceiveFileAsync(string pathNoName, string newName = "")
        {
            byte[] fileNameLengthData = await networkStreamHandler.ReadDataAsync(ProtocolSpecification.FileNameSize);
            int fileNameLength = BitConverter.ToInt32(fileNameLengthData);
            byte[] fileNameData = await networkStreamHandler.ReadDataAsync(fileNameLength);
            string fileName = Encoding.UTF8.GetString(fileNameData);
            if (!string.IsNullOrEmpty(newName))
            {
                fileName = newName;
            }
            byte[] fileSizeDataLength = await networkStreamHandler.ReadDataAsync(ProtocolSpecification.FileSize);
            long fileSize = BitConverter.ToInt64(fileSizeDataLength);
            await ReceiveFileAsync(fileSize, pathNoName + fileName);
        }

        private async Task SendFileAsync(long fileSize, string path)
        {
            long fileParts = ProtocolSpecification.CalculateParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart != fileParts)
                {
                    data = await _fileStreamHandler.ReadDataAsync(path, ProtocolSpecification.MaxPacketSize, offset);
                    offset += ProtocolSpecification.MaxPacketSize;
                }
                else
                {
                    int lastPartSize = (int)(fileSize - offset);
                    data = await _fileStreamHandler.ReadDataAsync(path, lastPartSize, offset);
                    offset += lastPartSize;
                }

                await networkStreamHandler.SendDataAsync(data);
                currentPart++;
            }
        }

        private async Task ReceiveFileAsync(long fileSize, string fileName)
        {
            long fileParts = ProtocolSpecification.CalculateParts(fileSize);
            long offset = 0;
            long currentPart = 1;
            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart != fileParts)
                {
                    data = await networkStreamHandler.ReadDataAsync(ProtocolSpecification.MaxPacketSize);
                    offset += ProtocolSpecification.MaxPacketSize;
                }
                else
                {
                    int lastPartSize = (int)(fileSize - offset);
                    data = await networkStreamHandler.ReadDataAsync(lastPartSize);
                    offset += lastPartSize;
                }
                await _fileStreamHandler.WriteDataAsync(fileName, data);
                currentPart++;
            }
        }
        public async Task<string> ReceiveMessageAsync()
        {
            byte[] dataLength = await networkStreamHandler.ReadDataAsync(ProtocolSpecification.FileNameSize);
            int length = BitConverter.ToInt32(dataLength);
            byte[] data = await networkStreamHandler.ReadDataAsync(length);
            string message = Encoding.UTF8.GetString(data);
            return message;
        }
        public async Task SendMessageAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            int length = data.Length;
            byte[] dataLength = BitConverter.GetBytes(length);

            await networkStreamHandler.SendDataAsync(dataLength);
            await networkStreamHandler.SendDataAsync(data);
        }
    }
}