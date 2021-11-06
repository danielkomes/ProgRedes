using Domain;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            string toSend = ProcessMessage(request.Name);
            if (toSend == null)
            {
                toSend = Logic.GameSeparator;
            }
            return Task.FromResult(new HelloReply
            {
                Message = toSend
            });
        }

        private string ProcessMessage(string message)
        {
            string ret = null;
            string action = message.Substring(0, message.IndexOf(Logic.GameTransferSeparator));
            message = message.Remove(0, action.Length + Logic.GameTransferSeparator.Length);

            if (action.Equals(ETransferType.Login.ToString()))
            {
                Client client = Sys.GetClient(message);
                if (client != null)
                {
                    if (!client.IsOnline)
                    {
                        client.IsOnline = true;
                        ret = bool.TrueString;
                        //await SendMessageAsync(fch, "true");
                    }
                    else
                    {
                        ret = bool.FalseString;
                        //await SendMessageAsync(fch, "false");
                    }
                }
                else
                {
                    ret = bool.FalseString;
                    //await SendMessageAsync(fch, "false");
                }
            }
            else if (action.Equals(ETransferType.Signup.ToString()))
            {
                bool msg = Sys.AddClient(message);
                if (msg)
                {
                    Client client = Sys.GetClient(message);
                    client.IsOnline = true;
                    //clients[tcpClient] = client;
                }
                //await SendMessageAsync(fch, msg + "");
                ret = msg.ToString();
            }
            else if (action.Equals(ETransferType.Logoff.ToString()))
            {
                Sys.GetClient(message).IsOnline = false;
            }
            else if (action.Equals(ETransferType.Publish.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                Sys.AddGame(game);
                //await ReceiveFileAsync(fch, game.Id + ".jpg");
                //TODO
            }
            else if (action.Equals(ETransferType.List.ToString()))
            {
                List<Game> list = Sys.GetGames();
                //await SendMessageAsync(fch, Logic.EncodeListGames(list));
                ret = Logic.EncodeListGames(list);
            }
            else if (action.Equals(ETransferType.Owned.ToString()))
            {
                Client c = Sys.GetClient(message);
                //await SendMessageAsync(fch, Logic.EncodeOwnedGames(c.OwnedGames));
                ret = Logic.EncodeOwnedGames(c.OwnedGames);
            }
            else if (action.Equals(ETransferType.Edit.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                Sys.ReplaceGame(game);
            }
            else if (action.Equals(ETransferType.Delete.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                Sys.DeleteGame(game);
            }
            else if (action.Equals(ETransferType.Review.ToString()))
            {
                string[] arr = message.Split(Logic.GameTransferSeparator);
                int id = int.Parse(arr[0]);
                Review r = Logic.DecodeReview(arr[1]);
                Sys.AddReview(id, r);
            }
            else if (action.Equals(ETransferType.Download.ToString()))
            {
                Game game = Logic.DecodeGame(message);
                int id = game.Id;
                //await SendFile(fch, ServerPosterFolder + id + ".jpg", game.Title + ".jpg");
                //TODO
            }
            else if (action.Equals(ETransferType.BuyGame.ToString()))
            {
                string[] arr = message.Split(Logic.GameTransferSeparator);
                int gameId = int.Parse(arr[0]);
                string username = arr[1];
                bool response = Sys.BuyGame(username, gameId);
                //await SendMessageAsync(fch, response + "");
                ret = response.ToString();
            }
            else if (action.Equals(ETransferType.Disconnect.ToString()))
            {
                //tcpClient.GetStream().Close();
                //clients.Remove(tcpClient);
                //Console.WriteLine("Client disconnected. Total: " + clients.Count);
                //ret = false;
            }
            return ret;
        }
    }
}
