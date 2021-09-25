using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class StringCommunicationHandler
    {
        private readonly Socket socket;

        public StringCommunicationHandler(Socket socket)
        {
            this.socket = socket;
        }
    }
}
