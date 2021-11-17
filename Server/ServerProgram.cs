using Domain;
using System.Threading.Tasks;

namespace Server
{
    class ServerProgram
    {
        static async Task Main(string[] args)
        {
            ServerHandler sh = new ServerHandler();
            await ServerConsole.ServerConsoleAsync(sh);
        }
    }
}
