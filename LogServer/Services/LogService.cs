using Grpc.Net.Client;
using LogHandler;
using LogServer.Interfaces;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LogHandler.LogExchange;

namespace LogServer.Services
{
    public class LogService : ILogService
    {
        private readonly LogExchangeClient loggerClient;
        public LogService()
        {
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:8001");
            loggerClient = new LogExchangeClient(channel);
        }

        public async Task<PaginatedResponse<LogEntry>> GetLogsAsync(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            GetLogsReply reply = await loggerClient.GetLogsAsync(
                new GetLogsRequest
                {
                    Page = page,
                    PageSize = pageSize
                }
            );
            List<LogEntry> logs = Logs.DecodeLogList(reply.List);
            int totalGames = Logs.LogList.Count;
            return PaginationHelper<LogEntry>.GeneratePaginatedResponse(pageSize, totalGames, logs);
        }
    }
}
