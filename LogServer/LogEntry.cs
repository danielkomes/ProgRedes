using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogServer
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
    }
}

