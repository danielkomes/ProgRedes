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
            logsLocker = new object();
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

        public static string EncodeLogList(List<LogEntry> list) 
        {
            string ret = "";
            foreach (LogEntry entry in list)
            {
                ret =  "|" + entry;
            }
            ret = ret[1..];
            return ret;
        }

        public static List<LogEntry> DecodeLogList(string st)
        {
            List<LogEntry> ret = new List<LogEntry>();
            string[] sp = st.Split("|");
            foreach (string entry in sp) 
            {
                if (!string.IsNullOrEmpty(entry))
                {
                    ret.Add(LogEntry.DecodeLogEntry(entry));
                }
            }
            return ret;
        }
    }
}