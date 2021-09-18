using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Domain
{
    public class ClientThread
    {
        //public HashCode hashCode { get; private set; }
        public Socket socket { get; set; }
        public string currentConsoleLocation { get; set; }
        public string locationRequest { get; set; }
        public string optionsResponse { get; set; }

        public ClientThread(Socket socket)
        {
            this.socket = socket;
            currentConsoleLocation = "0";
            locationRequest = "";
            optionsResponse = "";
        }
    }
}
