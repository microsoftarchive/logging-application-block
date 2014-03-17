using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT
{
    public static class MsmqUtil
    {
        public const string MessageQueuePath = @".\Private$\entlib";
        public const string MsgBody = "My message body";
        public const string MsgCategory = "foo";
        public const int MsgEventID = 1;
        public const string MsgTitle = "=== Header ===";

        public static void DeletePrivateTestQ()
        {
            string path = MessageQueuePath;
            if (MessageQueue.Exists(path))
            {
                MessageQueue.Delete(path);
            }
        }

        public static void ValidateMsmqIsRunning()
        {
            try
            {
                MessageQueue.Exists(MessageQueuePath);
            }
            catch (InvalidOperationException ex)
            {
                Assert.Inconclusive(ex.Message);
            }
        }

        public static void CreatePrivateTestQ()
        {
            string path = MessageQueuePath;
            if (MessageQueue.Exists(path))
            {
                DeletePrivateTestQ();
            }
            MessageQueue.Create(path, false);
        }

        public static LogEntry GetDefaultLogEntry()
        {
            LogEntry entry = new LogEntry(
                MsgBody,
                MsgCategory,
                -1,
                MsgEventID,
                TraceEventType.Information,
                MsgTitle,
                null);

            entry.Priority = 100;

            entry.TimeStamp = DateTime.MaxValue;
            entry.MachineName = "machine";

            return entry;
        }

        public static string GetLogEntryFromQueue()
        {
            using (MessageQueue msmq = new MessageQueue(MessageQueuePath))
            {
                Message[] messages = msmq.GetAllMessages();

                if (messages != null && messages.Length > 0)
                {
                    using (Message m = messages[0])
                    {
                        m.Formatter = new System.Messaging.XmlMessageFormatter(new String[] { });

                        using (StreamReader sr = new StreamReader(m.BodyStream))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }

            return null;
        }
    }
}
