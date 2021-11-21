using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogHandler
{
    public class LogEntry
    {
        public DateTime Date { get; set; }
        public ETransferType Action { get; set; }
        public string ClientName { get; set; }
        public Game AGame { get; set; }
        public Review AReview { get; set; }

        public string EncodeLogEntry()
        {
            string date = this.Date.ToString();
            string action = this.Action.ToString();
            string clientName = this.ClientName;
            string game = this.AGame != null ? Logic.EncodeGame(this.AGame) : "";

            string review = this.AReview != null ? Logic.EncodeReview(this.AReview) : "";
            string log = date + "@" + action + "@" + clientName + "@" + game + "@" + review; //To do: const string separator
            return log;
        }
        
        public static LogEntry DecodeLogEntry(string message)
        {
            string[] log = message.Split("@");
            string date = log[0];
            string action = log[1];
            string clientName = log[2];
            string game = log[3];
            string review = log[4];
            LogEntry Lg = new LogEntry();
            Lg.Date = Convert.ToDateTime(date);
            Lg.Action = (ETransferType)Enum.Parse(typeof (ETransferType), action);
            Lg.ClientName = clientName;

            if (!string.IsNullOrEmpty(game))
            {
                Lg.AGame = Logic.DecodeGame(game);
            }
            if (!string.IsNullOrEmpty(review))
            {
                Lg.AReview = Logic.DecodeReview(review);
            }

            return Lg;
        }
    }
}

