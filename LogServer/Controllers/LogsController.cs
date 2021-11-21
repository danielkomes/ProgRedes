using LogHandler;
using LogServer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogServer.Controllers
{
    [Route("logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogService logService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public LogsController(
            ILogService logService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<WebPaginatedResponse<LogEntry>>> GetLogs(int page = 1, int pageSize = 15)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest();
            }
            PaginatedResponse<LogEntry> logsPaginatedResponse = await logService.GetLogsAsync(page, pageSize);
            if (logsPaginatedResponse == null) //TODO: nunca va a ser nul
            {
                return NoContent();
            }

            string route = httpContextAccessor.HttpContext.Request.Host.Value +
                           httpContextAccessor.HttpContext.Request.Path;
            WebPaginatedResponse<LogEntry> response =
                WebPaginationHelper<LogEntry>.GenerateWebPaginatedResponse(logsPaginatedResponse, page, pageSize, route);

            return Ok(response);
        }
    }
}
