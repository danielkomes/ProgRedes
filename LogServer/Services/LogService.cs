using Grpc.Net.Client;
using LogHandler;
using LogServer.Interfaces;
using Newtonsoft.Json.Linq;
using Pagination;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static LogHandler.LogExchange;

namespace LogServer.Services
{
    public class LogService : ILogService
    {
        private readonly LogExchangeClient loggerClient;
        private string RpcAddress;
        public LogService()
        {
            ReadJson();
            GrpcChannel channel = GrpcChannel.ForAddress(RpcAddress);
            loggerClient = new LogExchangeClient(channel);
        }

        private void ReadJson()
        {
            string filepath = "LogServiceConfig.json";
            using StreamReader r = new StreamReader(filepath);
            var json = r.ReadToEnd();
            var jobj = JObject.Parse(json);
            RpcAddress = (string)jobj.GetValue("RpcAddress");
        }
        public async Task<PaginatedResponse<LogEntry>> GetLogsAsync(int gameId, string username, string minDate, string maxDate, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            GetLogsReply reply = await loggerClient.GetLogsAsync(
                new GetLogsRequest
                {
                    GameId = gameId,
                    Username = username,
                    MinDate = minDate,
                    MaxDate = maxDate,
                    Page = page,
                    PageSize = pageSize
                }
            );
            List<LogEntry> logs = Logs.DecodeLogList(reply.List);
            if (logs.Count == 0)
            {
                logs = null;
            }
            int totalGames = Logs.LogList.Count;
            return PaginationHelper<LogEntry>.GeneratePaginatedResponse(pageSize, totalGames, logs);
        }
    }
}
