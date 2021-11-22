using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdminServer;
using Domain;
using Grpc.Net.Client;
using Newtonsoft.Json.Linq;
using Pagination;
using WebApi.Interfaces;
using static AdminServer.MessageExchanger;

namespace WebApi.Services
{
    public class GameService : IGameService
    {
        private readonly MessageExchangerClient rpcClient;
        private string RpcAddress;

        public GameService()
        {
            ReadJson();
            GrpcChannel channel = GrpcChannel.ForAddress(RpcAddress);
            rpcClient = new MessageExchangerClient(channel);
        }
        private void ReadJson()
        {
            string filepath = "GameServiceConfig.json";
            using StreamReader r = new StreamReader(filepath);
            var json = r.ReadToEnd();
            var jobj = JObject.Parse(json);
            RpcAddress = (string)jobj.GetValue("RpcAddress");
        }

        public async Task<PaginatedResponse<Game>> GetGames(string title, string genre, int rating, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }

            PagedListReply reply = await rpcClient.ListPagedAsync(
                new PagedListRequest
                {
                    Title = title,
                    Genre = genre,
                    Rating = rating,
                    Page = page,
                    PageSize = pageSize
                });
            List<Game> games = Logic.DecodeListGames(reply.List);
            int totalGames = reply.TotalCount;
            return PaginationHelper<Game>.GeneratePaginatedResponse(pageSize, totalGames, games);
        }

        public async Task<Game> GetGameByIdAsync(int id)
        {
            MessageReply reply = await rpcClient.GetGameByIdAsync(
                new MessageRequest
                {
                    Message = id.ToString()
                });
            Game game = null;
            if (!string.IsNullOrEmpty(reply.Message))
            {
                game = Logic.DecodeGame(reply.Message);
            }
            return game;
        }
        public async Task<Game> PublishGameAsync(Game game)
        {
            PublishReply reply = await rpcClient.PublishAsync(
                new MessageRequest
                {
                    Message = Logic.EncodeGame(game)
                });
            game.Id = reply.Id;
            return game;
        }

        public async Task<Game> UpdateGameAsync(int id, Game game)
        {
            game.Id = id;
            MessageReply reply = await rpcClient.EditAsync(
                new MessageRequest
                {
                    Message = Logic.EncodeGame(game)
                });
            bool success = bool.Parse(reply.Message);
            if (success)
            {
                return game;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteGameAsync(int id)
        {
            Game game = await GetGameByIdAsync(id);
            if (game != null)
            {
                MessageReply reply = await rpcClient.DeleteAsync(
                    new MessageRequest
                    {
                        Message = Logic.EncodeGame(game)
                    });
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<PaginatedResponse<Review>> GetReviews(int id, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }

            PagedListReply reply = await rpcClient.ReviewsPagedAsync(
                new PagedListRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    Id = id
                });
            List<Review> reviews = Logic.DecodeReviews(reply.List);
            int totalReviews = reply.TotalCount;
            if (totalReviews == 0)
            {
                return null;
            }
            return PaginationHelper<Review>.GeneratePaginatedResponse(pageSize, totalReviews, reviews);
        }
        public async Task<Review> ReviewGameAsync(int id, Review review)
        {
            MessageReply reply = await rpcClient.ReviewAsync(
                new MessageRequest
                {
                    Message = id + Logic.GameTransferSeparator + Logic.EncodeReview(review)
                });
            bool success = bool.Parse(reply.Message);
            if (success)
            {
                return review;
            }
            else
            {
                return null;
            }
        }

    }
}