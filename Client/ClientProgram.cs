using System.Threading.Tasks;

namespace Client
{
    public class ClientProgram
    {
        static async Task Main(string[] args)
        {
            ClientHandler ch = await ClientHandler.ClientHandlerAsync();
            await ClientConsole.ClientConsoleAsync(ch);
        }
    }
}
