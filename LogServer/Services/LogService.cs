using LogServer.Interfaces;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogServer.Services
{
    public class LogService : ILogService
    {
        public PaginatedResponse<LogEntry> GetLogs(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }
            List<LogEntry> logs = Logs.GetLogs(page, pageSize);
            int totalGames = Logs.LogList.Count;
            return PaginationHelper<LogEntry>.GeneratePaginatedResponse(pageSize, totalGames, logs);
        }
    }
}
