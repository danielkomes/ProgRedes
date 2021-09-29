namespace Client
{
    public class ClientProgram
    {
        static void Main(string[] args)
        {
            ClientHandler ClientHandler = new ClientHandler();
            new ClientConsole(ClientHandler);
        }

    }
}
