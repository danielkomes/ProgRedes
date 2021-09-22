using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Domain
{
    public class ClientThread
    {
        public Socket Socket { get; set; }
        public string CurrentConsoleLocation { get; set; }
        public string LocationRequest { get; set; }
        public string OptionsResponse { get; set; }
        public Game GameToPublish { get; set; }
        public Game GameToView { get; set; }

        public ClientThread(Socket socket)
        {
            this.Socket = socket;
            CurrentConsoleLocation = "0";
            LocationRequest = "";
            OptionsResponse = "";
        }
    }
}
