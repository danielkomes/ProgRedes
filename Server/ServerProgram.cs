using Domain;

namespace Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            ServerHandler sh = new ServerHandler();
            new ServerConsole(sh);
        }
    }
}
