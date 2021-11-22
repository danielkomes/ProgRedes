using AdminServer;
using Domain;
using Grpc.Net.Client;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaces;
using static AdminServer.MessageExchanger;

namespace WebApi.Services
{
    public class ClientService : IClientService
    {
        private readonly MessageExchangerClient client;

        public ClientService()
        {
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:4001"); //TODO: move to ServerConfig.json
            client = new MessageExchangerClient(channel);
        }

        public async Task<PaginatedResponse<Client>> GetClients(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }

            PagedListReply reply = await client.ListClientsPagedAsync(
                new PagedListRequest
                {
                    Page = page,
                    PageSize = pageSize
                });
            List<Client> clients = Logic.DecodeListClients(reply.List);
            int totalGames = reply.TotalCount;
            if (clients.Count == 0)
            {
                clients = null;
            }
            return PaginationHelper<Client>.GeneratePaginatedResponse(pageSize, totalGames, clients);
        }
    }
}
