using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    class ClientThread
    {
        public HashCode hashCode { get; private set; }
        public int currentConsoleLocation { get; set; }
        public int nextLocationQueued { get; set; }
    }
}
