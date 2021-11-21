using Domain;
using System;

namespace LogHandler
{
    public class LogEntry
    {
        private const string LOG_DATA_SEPARATOR = "@";
        public DateTime Date { get; set; }
        public ETransferType Action { get; set; }
        public string Username { get; set; }
        public Game Game { get; set; }
        public Review Review { get; set; }

        public string EncodeLogEntry()
        {
            string date = Date.ToString();
            string action = Action.ToString();
            string clientName = Username;
            string game = Game != null ? Logic.EncodeGame(Game) : "";

            string review = Review != null ? Logic.EncodeReview(Review) : "";
            string log = date + LOG_DATA_SEPARATOR + action + LOG_DATA_SEPARATOR + clientName + LOG_DATA_SEPARATOR + game + LOG_DATA_SEPARATOR + review;
            return log;
        }

        public static LogEntry DecodeLogEntry(string message)
        {
            string[] arr = message.Split(LOG_DATA_SEPARATOR);
            string date = arr[0];
            string action = arr[1];
            string clientName = arr[2];
            string game = arr[3];
            string review = arr[4];
            LogEntry log = new LogEntry();
            log.Date = Convert.ToDateTime(date);
            log.Action = (ETransferType)Enum.Parse(typeof(ETransferType), action);
            log.Username = clientName;

            if (!string.IsNullOrEmpty(game))
            {
                log.Game = Logic.DecodeGame(game);
            }
            if (!string.IsNullOrEmpty(review))
            {
                log.Review = Logic.DecodeReview(review);
            }

            return log;
        }
    }
}

