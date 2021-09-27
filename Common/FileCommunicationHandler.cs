using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class FileCommunicationHandler
    {
        static void Main(string[] args)
        {

        }
        private readonly FileStreamHandler _fileStreamHandler;
        private readonly SocketStreamHandler _socketStreamHandler;

        public FileCommunicationHandler(Socket socket)
        {
            _socketStreamHandler = new SocketStreamHandler(socket);
            _fileStreamHandler = new FileStreamHandler();
        }

        public void SendFile(string path)
        {
            var fileInfo = new FileInfo(path);
            string fileName = fileInfo.Name;
            byte[] fileNameData = Encoding.UTF8.GetBytes(fileName);
            int fileNameLength = fileNameData.Length;
            byte[] fileNameLengthData = BitConverter.GetBytes(fileNameLength);
            // 1.- Envío el largo del nombre del archivo
            _socketStreamHandler.SendData(fileNameLengthData);
            // 2.- Envío el nombre del archivo
            _socketStreamHandler.SendData(fileNameData);

            long fileSize = fileInfo.Length;
            byte[] fileSizeDataLength = BitConverter.GetBytes(fileSize);
            // 3.- Envío el largo del archivo
            _socketStreamHandler.SendData(fileSizeDataLength);
            // 4.- Envío los datos del archivo
            SendFile(fileSize, path);
        }

        public void ReceiveFile(string pathNoName)
        {
            // 1.- Recibir el largo del nombre del archivo
            byte[] fileNameLengthData = _socketStreamHandler.ReceiveData(ProtocolSpecification.FileNameSize);
            int fileNameLength = BitConverter.ToInt32(fileNameLengthData);
            // 2.- Recibir el nombre del archivo
            byte[] fileNameData = _socketStreamHandler.ReceiveData(fileNameLength);
            string fileName = Encoding.UTF8.GetString(fileNameData);

            // 3.- Recibo el largo del archivo
            byte[] fileSizeDataLength = _socketStreamHandler.ReceiveData(ProtocolSpecification.FileSize);
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
                    data = _socketStreamHandler.ReceiveData(ProtocolSpecification.MaxPacketSize);
                    offset += ProtocolSpecification.MaxPacketSize;
                }
                else
                {
                    int lastPartSize = (int)(fileSize - offset);
                    data = _socketStreamHandler.ReceiveData(lastPartSize);
                    offset += lastPartSize;
                }
                _fileStreamHandler.WriteData(fileName, data);
                currentPart++;
            }
        }
        public string ReceiveMessage()
        {
            byte[] dataLength = _socketStreamHandler.ReceiveData(ProtocolSpecification.FileNameSize);
            // 3 Interpreto ese valor para obtener el largo variable
            int length = BitConverter.ToInt32(dataLength);
            // 4 Creo el buffer para leer los datos
            // 5 Recibo los datos
            byte[] data = _socketStreamHandler.ReceiveData(length);
            // 6 Convierto (decodifico) esos datos a un string
            string message = Encoding.UTF8.GetString(data);
            // 7 Muestro los datos
            return message;
        }
        public void SendMessage(string message)
        {
            // 2 Codifico el mensaje a bytes
            byte[] data = Encoding.UTF8.GetBytes(message);
            // 3 Leo el largo de los datos codificados a bytes
            int length = data.Length;
            // 4 Codifico el largo de los datos variables a bytes
            byte[] dataLength = BitConverter.GetBytes(length);
            // 6 Envío el mensaje codificado a bytes

            _socketStreamHandler.SendData(dataLength);
            _socketStreamHandler.SendData(data);
        }
    }
}