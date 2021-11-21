using LogHandler;
using Pagination;
using System.Threading.Tasks;

namespace LogServer.Interfaces
{
    public interface ILogService
    {
        Task<PaginatedResponse<LogEntry>> GetLogsAsync(int gameId, string username, string minDate, string maxDate, int page, int pageSize);
    }
}
