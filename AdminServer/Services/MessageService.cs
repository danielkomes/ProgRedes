using Domain;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.Services
{
    public class MessageService : MessageExchanger.MessageExchangerBase
    {

        public override Task<MessageReply> Login(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;
            string reply = "";

            Client client = Sys.GetClient(message);
            if (client != null)
            {
                if (!client.IsOnline)
                {
                    client.IsOnline = true;
                    //clients[tcpClient] = client;
                    //await SendMessageAsync(fch, "true");
                    reply = bool.TrueString;
                }
                else
                {
                    //await SendMessageAsync(fch, "false");
                    reply = bool.FalseString;
                }
            }
            else
            {
                //await SendMessageAsync(fch, "false");
                reply = bool.FalseString;
            }
            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override Task<MessageReply> Signup(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;

            bool msg = Sys.AddClient(message);

            string reply = msg.ToString();

            if (msg)
            {
                Client client = Sys.GetClient(message);
                client.IsOnline = true;
                //clients[tcpClient] = message;
            }
            //await SendMessageAsync(fch, msg + "");
            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override Task<MessageReply> Logoff(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;
            string reply = "";

            Sys.GetClient(message).IsOnline = false;

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }

        public override Task<PublishReply> Publish(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;
            string reply = "";

            Game game = Logic.DecodeGame(message);
            Sys.AddGame(game);

            reply = game.Id + "";

            //await ReceiveFileAsync(fch, game.Id + ".jpg");

            return Task.FromResult(new PublishReply
            {
                Id = reply,
                Title = game.Title
            });
        }
        public override async Task<MessageReply> ReceiveFile(FileExchange request, ServerCallContext context)
        {
            string fileName = request.FileName;
            byte[] fileData = request.FileData.ToByteArray();
            string reply = "";

            await using FileStream fileStream = new FileStream("Posters/" + fileName + ".jpg", FileMode.Create);
            await fileStream.WriteAsync(fileData, 0, fileData.Length);

            //await ReceiveFileAsync(fch, game.Id + ".jpg");

            return await Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }

        public override Task<MessageReply> List(MessageRequest request, ServerCallContext context)
        {
            List<Game> list = Sys.GetGames();
            string reply = Logic.EncodeListGames(list);
            //await SendMessageAsync(fch, Logic.EncodeListGames(list));

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override Task<MessageReply> Owned(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;
            Client c = Sys.GetClient(message);
            string reply = Logic.EncodeOwnedGames(c.OwnedGames);
            //await SendMessageAsync(fch, Logic.EncodeListGames(list));

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override Task<MessageReply> Edit(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;
            Game game = Logic.DecodeGame(message);
            Sys.ReplaceGame(game);
            string reply = "";

            //await SendMessageAsync(fch, Logic.EncodeListGames(list));

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override Task<MessageReply> Delete(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;
            Game game = Logic.DecodeGame(message);
            Sys.DeleteGame(game);
            string reply = "";

            //await SendMessageAsync(fch, Logic.EncodeListGames(list));

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override Task<MessageReply> Review(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;

            string[] arr = message.Split(Logic.GameTransferSeparator);
            int id = int.Parse(arr[0]);
            Review r = Logic.DecodeReview(arr[1]);
            Sys.AddReview(id, r);

            string reply = "";

            //await SendMessageAsync(fch, Logic.EncodeListGames(list));

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override async Task<FileExchange> Download(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;

            Game game = Logic.DecodeGame(message);
            int gameId = game.Id;
            byte[] fileData = await File.ReadAllBytesAsync("Posters/" + gameId + ".jpg");
            //await SendFile(fch, ServerPosterFolder + id + ".jpg", game.Title + ".jpg");

            return await Task.FromResult(new FileExchange
            {
                FileId = gameId.ToString(),
                FileName = game.Title,
                FileData = Google.Protobuf.ByteString.CopyFrom(fileData)
            });
        }
        public override Task<MessageReply> BuyGame(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;

            string[] arr = message.Split(Logic.GameTransferSeparator);
            int gameId = int.Parse(arr[0]);
            string username = arr[1];
            bool response = Sys.BuyGame(username, gameId);

            return Task.FromResult(new MessageReply
            {
                Message = response.ToString()
            });
        }
    }
}
