using Domain;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LogHandler
{
    public class LogServiceRpc : LogExchange.LogExchangeBase
    {
        private readonly ILogger<LogServiceRpc> _logger;
        public LogServiceRpc(ILogger<LogServiceRpc> logger)
        {
            _logger = logger;
        }

        public override Task<GetLogsReply> GetLogs(GetLogsRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetLogsReply
            {
                List = Logs.EncodeLogList(Logs.GetLogs(request.GameId, request.Username, request.MinDate, request.MaxDate, request.Page, request.PageSize))
            });
        }

        public override Task<SaveLogReply> SaveLog(SaveLogRequest request, ServerCallContext context)
        {
            LogEntry entry = new LogEntry
            {
                Date = DateTime.Parse(request.Date),
                Username = request.Username,
                Action = (ETransferType)Enum.Parse(typeof(ETransferType), request.Action),
                Game = !string.IsNullOrEmpty(request.Game) ? Logic.DecodeGame(request.Game) : null,
                Review = !string.IsNullOrEmpty(request.Review) ? Logic.DecodeReview(request.Review) : null
            };

            Logs.Add(entry);

            return Task.FromResult(new SaveLogReply
            {
                Message = true
            });
        }
    }
}
