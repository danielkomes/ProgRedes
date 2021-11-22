using AdminServer;
using Domain;
using Grpc.Net.Client;
using Newtonsoft.Json.Linq;
using Pagination;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaces;
using static AdminServer.MessageExchanger;

namespace WebApi.Services
{
    public class ClientService : IClientService
    {
        private readonly MessageExchangerClient rpcClient;
        private string RpcAddress;

        public ClientService()
        {
            ReadJson();
            GrpcChannel channel = GrpcChannel.ForAddress(RpcAddress);
            rpcClient = new MessageExchangerClient(channel);
        }
        private void ReadJson()
        {
            string filepath = "ClientServiceConfig.json";
            using StreamReader r = new StreamReader(filepath);
            var json = r.ReadToEnd();
            var jobj = JObject.Parse(json);
            RpcAddress = (string)jobj.GetValue("RpcAddress");
        }

        public async Task<PaginatedResponse<Client>> GetClients(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }

            PagedListReply reply = await rpcClient.ListClientsPagedAsync(
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
