using System;
using System.Collections.Generic;

namespace LogHandler
{
    public static class Logs
    {
        private const string LOG_SEPARATOR = "|";
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
        public static List<LogEntry> GetLogs(int gameId, string username, string minDate, string maxDate, int page, int pageSize)
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
                            LogEntry entry = LogList[i];
                            bool add = true;

                            if (add && gameId >= 0)
                            {
                                if (entry.Game != null && entry.Game.Id == gameId)
                                {
                                    add &= true;
                                }
                                else
                                {
                                    add &= false;
                                }
                            }
                            if (add && !string.IsNullOrEmpty(username))
                            {
                                if (entry.Username.Equals(username))
                                {
                                    add &= true;
                                }
                                else
                                {
                                    add &= false;
                                }
                            }
                            if (add && !string.IsNullOrEmpty(minDate))
                            {
                                if (DateTime.TryParse(minDate, out DateTime dateTime))
                                {
                                    if (entry.Date.CompareTo(dateTime) >= 0)
                                    {
                                        add &= true;
                                    }
                                    else
                                    {
                                        add &= false;
                                    }
                                }
                            }
                            if (add && !string.IsNullOrEmpty(maxDate))
                            {
                                if (DateTime.TryParse(maxDate, out DateTime dateTime))
                                {
                                    if (entry.Date.CompareTo(dateTime) <= 0)
                                    {
                                        add &= true;
                                    }
                                    else
                                    {
                                        add &= false;
                                    }
                                }
                            }
                            if (add)
                            {
                                ret.Add(entry);
                            }
                        }
                    }
                }
                return ret;
            }
        }

        public static string EncodeLogList(List<LogEntry> list)
        {
            string ret = "";
            if (list.Count > 0)
            {
                foreach (LogEntry entry in list)
                {
                    ret += LOG_SEPARATOR + entry.EncodeLogEntry();
                }
                ret = ret[1..];
            }
            return ret;
        }

        public static List<LogEntry> DecodeLogList(string st)
        {
            List<LogEntry> ret = new List<LogEntry>();
            string[] sp = st.Split(LOG_SEPARATOR);
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