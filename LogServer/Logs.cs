using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogServer
{
    public static class Logs
    {
        public static List<LogEntry> LogList;
        private static object logsLocker;

        static Logs()
        {
            LogList = new List<LogEntry>();
        }

        public static void Add(LogEntry log)
        {
            lock (logsLocker)
            {
                LogList.Add(log);
            }
        }
        public static List<LogEntry> GetLogs(int page, int pageSize)
        {
            lock (logsLocker)
            {
                List<LogEntry> ret = new List<LogEntry>();
                if (pageSize > 0 && page > 0)
                {
                    int count = LogList.Count;
                    int start = page * pageSize - pageSize;
                    int end = Math.Min(start + pageSize, count);
                    if (start >= 0 && start < count)
                    {
                        for (int i = start; i < end; i++)
                        {
                            ret.Add(LogList[i]);
                        }
                    }
                }
                return ret;
            }
        }
    }
}