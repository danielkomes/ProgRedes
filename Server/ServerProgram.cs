using Domain;

namespace Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            Logic.TestGames();
            ServerHandler sh = new ServerHandler();
            new ServerConsole(sh);
        }
    }
}
