using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoutedPublisher
{
    public class LogEntry
    {
        public DateTime Date { get; set; }
        public ETransferType Action { get; set; }
        public string ClientName { get; set; }
        public Game AGame { get; set; }
        public Review AReview { get; set; }
    }
}

