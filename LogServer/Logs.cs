using Domain;
using RoutedPublisher;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogServer
{
    public static class Logs
    {
        public static List<LogEntry> LogList;
        static Logs() {
            LogList = new List<LogEntry>();
        } 

        public static void Add(LogEntry log) 
        {
            LogList.Add(log);
        }

    }
}