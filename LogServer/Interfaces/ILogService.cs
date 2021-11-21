using LogHandler;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogServer.Interfaces
{
    public interface ILogService
    {
        Task <PaginatedResponse<LogEntry>> GetLogsAsync(int page, int pageSize);
    }
}
