using Domain;
using Grpc.Core;
using LogServer;
using RoutedPublisher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.Services
{
    public class MessageService : MessageExchanger.MessageExchangerBase
    {
        private const string POSTERS = "bin/Debug/netcoreapp3.1/Posters/";
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

                    LogEntry log = new LogEntry()
                    {
                        Date = DateTime.Now,
                        Action = ETransferType.Login,
                        ClientName = request.Username,
                        AGame = null,
                        AReview = null
                    };
                    Logs.Add(log);
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

                LogEntry log = new LogEntry()
                {
                    Date = DateTime.Now,
                    Action = ETransferType.Signup,
                    ClientName = request.Username,
                    AGame = null,
                    AReview = null
                };
                Logs.Add(log);

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

            LogEntry log = new LogEntry()
            {
                Date = DateTime.Now,
                Action = ETransferType.Logoff,
                ClientName = request.Username,
                AGame = null,
                AReview = null
            };
            Logs.Add(log);

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }

        public override Task<PublishReply> Publish(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;

            Game game = Logic.DecodeGame(message);
            Sys.AddGame(game);
            LogEntry log = new LogEntry()
            {
                Date = DateTime.Now,
                Action = ETransferType.Publish,
                ClientName = request.Username,
                AGame = game,
                AReview = null
            };
            Publisher.PublishMessage(log);

            //await ReceiveFileAsync(fch, game.Id + ".jpg");

            return Task.FromResult(new PublishReply
            {
                Id = game.Id,
                Title = game.Title
            });
        }
        public override async Task<MessageReply> ReceiveFile(FileExchange request, ServerCallContext context)
        {
            string fileName = request.FileName;
            byte[] fileData = request.FileData.ToByteArray();
            string reply = "";

            try
            {
                await using FileStream fileStream = new FileStream(POSTERS + fileName + ".jpg", FileMode.Create);
                await fileStream.WriteAsync(fileData, 0, fileData.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

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
        public override Task<PagedListReply> ListPaged(PagedListRequest request, ServerCallContext context)
        {
            List<Game> list = Sys.GetGames(request.Page, request.PageSize);
            string reply = Logic.EncodeListGames(list);
            //await SendMessageAsync(fch, Logic.EncodeListGames(list));

            return Task.FromResult(new PagedListReply
            {
                List = reply,
                TotalCount = Sys.Games.Count
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
            string reply = Sys.ReplaceGame(game).ToString();

            LogEntry log = new LogEntry()
            {
                Date = DateTime.Now,
                Action = ETransferType.Edit,
                ClientName = request.Username,
                AGame = game,
                AReview = null
            };
            Logs.Add(log);

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

            LogEntry log = new LogEntry()
            {
                Date = DateTime.Now,
                Action = ETransferType.Delete,
                ClientName = request.Username,
                AGame = game,
                AReview = null
            };
            Logs.Add(log);

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
            Game game = Sys.GetGame(id);
            Review r = Logic.DecodeReview(arr[1]);
            Sys.AddReview(id, r);

            string reply = "";

            LogEntry log = new LogEntry()
            {
                Date = DateTime.Now,
                Action = ETransferType.Review,
                ClientName = request.Username,
                AGame = game,
                AReview = r
            };
            Logs.Add(log);

            //await SendMessageAsync(fch, Logic.EncodeListGames(list));

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override Task<PagedListReply> ReviewsPaged(PagedListRequest request, ServerCallContext context)
        {
            List<Review> reviews = Sys.GetReviews(request.Id, request.Page, request.PageSize);
            string reply = Logic.EncodeReviews(reviews);
            return Task.FromResult(new PagedListReply
            {
                List = reply,
                TotalCount = Sys.GetGame(request.Id).Reviews.Count
            });
        }
        public override async Task<FileExchange> Download(MessageRequest request, ServerCallContext context)
        {
            string message = request.Message;

            Game game = Logic.DecodeGame(message);
            int gameId = game.Id;
            byte[] fileData = await File.ReadAllBytesAsync(POSTERS + gameId + ".jpg");
            //await SendFile(fch, ServerPosterFolder + id + ".jpg", game.Title + ".jpg");

            LogEntry log = new LogEntry()
            {
                Date = DateTime.Now,
                Action = ETransferType.Download,
                ClientName = request.Username,
                AGame = game,
                AReview = null
            };
            Logs.Add(log);

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
            Game game = Sys.GetGame(gameId);
            string username = arr[1];
            bool response = Sys.BuyGame(username, gameId);

            LogEntry log = new LogEntry()
            {
                Date = DateTime.Now,
                Action = ETransferType.BuyGame,
                ClientName = request.Username,
                AGame = game,
                AReview = null
            };
            Logs.Add(log);

            return Task.FromResult(new MessageReply
            {
                Message = response.ToString()
            });
        }

        public override Task<MessageReply> ListClients(MessageRequest request, ServerCallContext context)
        {
            List<Client> list = Sys.GetClients();
            string reply = Logic.EncodeListClients(list);

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override Task<MessageReply> RemoveGameFromClient(RemoveGameFromClientRequest request, ServerCallContext context)
        {
            Sys.RemoveGameFromClient(request.Username, request.GameId);
            string reply = "";

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override Task<MessageReply> RemoveAllGamesFromClient(MessageRequest request, ServerCallContext context)
        {
            Sys.RemoveAllGamesFromClient(request.Message);
            string reply = "";

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
        public override Task<MessageReply> DeleteClient(MessageRequest request, ServerCallContext context)
        {
            Sys.DeleteClient(request.Message);
            string reply = "";

            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }

        public override Task<MessageReply> GetGameById(MessageRequest request, ServerCallContext context)
        {
            Game game = Sys.GetGame(int.Parse(request.Message));
            string reply = "";
            if (game != null)
            {
                reply = Logic.EncodeGame(game);
            }
            return Task.FromResult(new MessageReply
            {
                Message = reply
            });
        }
    }
}
