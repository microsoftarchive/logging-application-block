using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT
{
    public class LogEntryFactory
    {
        public const int DefaultEventId = 1;
        public const int DefaultPriority = 1;
        public const string DefaultMessage = "Sample Message.";

        public static LogEntry GetLogEntry()
        {
            LogEntry logEntry = new LogEntry();
            logEntry.EventId = DefaultEventId;
            logEntry.Priority = DefaultPriority;
            logEntry.Message = DefaultMessage;
            logEntry.Categories.Clear();
            logEntry.Categories = new List<string>() { "Trace", "Debug" };

            return logEntry;
        }

        public static LogEntry GetLogEntry(string category, string message)
        {
            LogEntry logEntry = new LogEntry();
            logEntry.EventId = DefaultEventId;
            logEntry.Priority = DefaultPriority;
            logEntry.Message = message;
            logEntry.Categories.Clear();
            logEntry.Categories.Add(category);

            return logEntry;
        }
    }
}