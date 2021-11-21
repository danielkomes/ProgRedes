using Grpc.Net.Client;
using LogHandler;
using LogServer.Interfaces;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LogHandler.Logger;

namespace LogServer.Services
{
    public class LogService : ILogService
    {
        private readonly LoggerClient loggerClient;
        public LogService()
        {
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:8001");
            loggerClient = new LoggerClient(channel);
        }

        public async Task<PaginatedResponse<LogEntry>> GetLogs(int page, int pageSize)
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
