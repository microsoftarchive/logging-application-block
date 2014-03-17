using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Json
{
    [TestClass]
    public class JsonLogFormatterFixture : LoggingFixtureBase
    {
        private string fromaddress = "from@127.0.0.1.email.address";
        private string toaddress = "to@127.0.0.1.email.address";
        private string smtpserver = "localhost";
        private string userName = String.Empty;
        private string password = String.Empty;

        // DeafultLog entry property constants
        private const string MsgBody = "My message body";
        private const string MsgCategory = "foo";
        private const int MsgEventID = 1;
        private const string MsgTitle = "=== Header ===";
        private string fileNameWithoutExtension;
        private string fileName;
        private const string Extension = ".log";

        [TestInitialize]
        public override void TestInitialize()
        {
            LogFileReader.CreateDirectory("mail");
        }

        [TestCleanup]
        public override void Cleanup()
        {
            if (this.fileName != null)
            {
                foreach (string createdFileName in Directory.GetFiles(".", this.fileNameWithoutExtension + "*"))
                {
                    File.Delete(createdFileName);
                }
            }
        }

        // Check JsonFormatter in RollingFlatFile
        [TestMethod]
        public void RollIsNecessaryWhenUsingJsonFormatterWithRollingFlatFileTraceListener()
        {
            this.fileNameWithoutExtension = Guid.NewGuid().ToString();
            this.fileName = this.fileNameWithoutExtension + JsonLogFormatterFixture.Extension;

            using (RollingFlatFileTraceListener traceListener
                 = new RollingFlatFileTraceListener(this.fileName, "header", "footer", new JsonLogFormatter(JsonFormatting.Indented),
                                                    1, "yyyy", RollFileExistsBehavior.Increment, RollInterval.Year))
            {
                traceListener.RollingHelper.UpdateRollingInformationIfNecessary();

                traceListener.Write(new string('c', 1200));

                Assert.IsNotNull(traceListener.RollingHelper.CheckIsRollNecessary());
            }
        }

        //// Check JsonFormatter in FlatFile
        [TestMethod]
        public void MessageIsWrittenWhenUsingJsonFormatterWithFlatFileTraceListener()
        {
            this.fileNameWithoutExtension = Guid.NewGuid().ToString();
            this.fileName = this.fileNameWithoutExtension + JsonLogFormatterFixture.Extension;

            using (FlatFileTraceListener tracelistener = new FlatFileTraceListener(this.fileName, null, null, new JsonLogFormatter(JsonFormatting.Indented)))
            {
                tracelistener.TraceData(new TraceEventCache(), "source", TraceEventType.Error, 1, "This is a test");
                tracelistener.Dispose();
                Assert.IsTrue(File.Exists(this.fileName));

                string strLog = LogFileReader.ReadFileWithoutLock(this.fileName);
                StringAssert.Contains(strLog, "source Error: 1 : This is a test\r\n");
            }
        }

        //// Check JsonFormatter in MsmqTraceListener
        [TestMethod]
        public void MessageIsDeserializedWhenUsingJsonFormatterWithMsmqTraceListener()
        {
            MsmqTraceListener listener =
                new MsmqTraceListener("unnamed", MsmqUtil.MessageQueuePath, new JsonLogFormatter(), MessagePriority.Low, true,
                                      MsmqTraceListenerData.DefaultTimeToBeReceived, MsmqTraceListenerData.DefaultTimeToReachQueue,
                                      false, false, false, MessageQueueTransactionType.None);
            LogEntry entry = MsmqUtil.GetDefaultLogEntry();

            Message message = listener.CreateMessage(entry);
            Assert.IsNotNull(message);
            Assert.IsNotNull(message.Body);
            Assert.AreEqual(message.Body.GetType(), typeof(string));

            LogEntry deserializedEntry = JsonLogFormatter.Deserialize<LogEntry>(message.Body as string);
            Assert.IsNotNull(deserializedEntry);
            Assert.AreEqual(entry.Message, deserializedEntry.Message);
        }

        [TestMethod]
        public void JsonFormatterHeadersAreSetWhenSendingEmail()
        {
            DateTime messageTimestamp = DateTime.UtcNow;

            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();

            JsonLogFormatter jsonformatter = new JsonLogFormatter(JsonFormatting.Indented);

            EmailTraceListener emailListener = new EmailTraceListener(this.toaddress,
                                            this.fromaddress,
                                            "StartOfSubject",
                                            "EndOfSubject", this.smtpserver, jsonformatter);

            emailListener.Filter = new EventTypeFilter(SourceLevels.All);
            loggingConfiguration.AddLogSource("Email", SourceLevels.All, true, emailListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(emailListener);

            string message = "Test JSON";

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write(message, "General");
            this.writer.Dispose();

            string emailText = LogFileReader.GetEmail();
            string endOfHeader = "\r\n\r\n";
            int index = emailText.IndexOf(endOfHeader);
            string header = emailText.Substring(0, index);

            Dictionary<string, string> emailDictionary = Regex.Split(header, "\r\n").Select(e => e.Split(':')).ToDictionary(line => line[0], line => line[1].Trim());

            Assert.AreEqual("StartOfSubject Information EndOfSubject", emailDictionary["Subject"]);
            Assert.AreEqual(this.fromaddress, emailDictionary["From"]);
            Assert.AreEqual(this.toaddress, emailDictionary["To"]);
        }

        [TestMethod]
        public void MessageIsSetWhenUsingJsonFormatter()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();

            JsonLogFormatter jsonformatter = new JsonLogFormatter(JsonFormatting.Indented);

            EmailTraceListener emailListener = new EmailTraceListener(this.toaddress,
                                            this.fromaddress,
                                            "StartOfSubject",
                                            "EndOfSubject", this.smtpserver, jsonformatter);

            emailListener.Filter = new EventTypeFilter(SourceLevels.All);
            loggingConfiguration.AddLogSource("Email", SourceLevels.All, true, emailListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(emailListener);

            string message = "Test JSON";

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write(message, "General");
            this.writer.Dispose();

            LogEntry logEntry = LogFileReader.GetLogEntryFromEmail();

            Assert.IsTrue(logEntry.Message == message);
        }

        //// Check JsonFormatter-Indented in FormattedEventLogTraceListener
        [TestMethod]
        public void EntryIsWrittenWithIndentedFormatWhenUsingJsonFormatterWithFormattedEventLogTraceListener()
        {
            var eventLog = new EventLog(LoggingFixtureBase.EventLogName, ".", "Enterprise Library Logging");

            FormattedEventLogTraceListener listener =
                new FormattedEventLogTraceListener(eventLog, new JsonLogFormatter(JsonFormatting.Indented));

            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            loggingConfiguration.AddLogSource("notfromconfig", SourceLevels.All, true, listener);

            LogEntry logEntry = MsmqUtil.GetDefaultLogEntry();
            logEntry.Categories = new string[] { "notfromconfig" };

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write(logEntry);

            EventLogEntry eventLogEntry = GetLastEventLogEntry();

            Assert.IsTrue(eventLogEntry.Message.IndexOf("\r\n  \"Message\": \"My message body\",\r\n  \"Categories\": [\r\n    \"notfromconfig\"\r\n  ],\r\n  \"Priority\": 100,\r\n  \"EventId\": 1,\r\n  \"Severity\": 8,\r\n  \"LoggedSeverity\": \"Information\",\r\n  \"Title\": \"=== Header ===\",\r\n  \"TimeStamp\": ") != -1);
        }

        //// Check JsonFormatter-None in FormattedEventLogTraceListener
        [TestMethod]
        public void EntryIsWrittenWithFormattingNoneWhenUsingJsonFormatterWithFormattedEventLogTraceListener()
        {
            var eventLog = new EventLog(LoggingFixtureBase.EventLogName, ".", "Enterprise Library Logging");

            FormattedEventLogTraceListener listener =
                new FormattedEventLogTraceListener(eventLog, new JsonLogFormatter(JsonFormatting.None));

            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            loggingConfiguration.AddLogSource("notfromconfig", SourceLevels.All, true, listener);

            LogEntry logEntry = MsmqUtil.GetDefaultLogEntry();
            logEntry.Categories = new string[] { "notfromconfig" };

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write(logEntry);

            EventLogEntry eventLogEntry = GetLastEventLogEntry();

            Assert.IsTrue(eventLogEntry.Message.IndexOf("\"Message\":\"My message body\",\"Categories\":[\"notfromconfig\"],\"Priority\":100,\"EventId\":1,\"Severity\":8,\"LoggedSeverity\":\"Information\",\"Title\":\"=== Header ===\",\"TimeStamp\":") != -1);
        }

        //// Check JsonFormatter-NoFormatting in FormattedEventLogTraceListener
        [TestMethod]
        public void EntryIsWrittenWithNoFormattingWhenUsingJsonFormatterWithFormattedEventLogTraceListener()
        {
            var eventLog = new EventLog(LoggingFixtureBase.EventLogName, ".", "Enterprise Library Logging");

            FormattedEventLogTraceListener listener =
                new FormattedEventLogTraceListener(eventLog, new JsonLogFormatter());

            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            loggingConfiguration.AddLogSource("notfromconfig", SourceLevels.All, true, listener);

            LogEntry logEntry = MsmqUtil.GetDefaultLogEntry();
            logEntry.Categories = new string[] { "notfromconfig" };

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write(logEntry);

            EventLogEntry eventLogEntry = GetLastEventLogEntry();

            Assert.IsTrue(eventLogEntry.Message.IndexOf("\"Message\":\"My message body\",\"Categories\":[\"notfromconfig\"],\"Priority\":100,\"EventId\":1,\"Severity\":8,\"LoggedSeverity\":\"Information\",\"Title\":\"=== Header ===\",\"TimeStamp\":") != -1);
        }

        //// Check JsonFormatter-Indented in FormattedTextWriterTraceListener
        [TestMethod]
        public void MessageIsFormattedWhenUsingJsonFormatterWithFormattedTextWriterTraceListener()
        {
            using (StringWriter writer = new StringWriter())
            {
                FormattedTextWriterTraceListener listener = new FormattedTextWriterTraceListener(writer, new JsonLogFormatter(JsonFormatting.Indented));

                LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
                loggingConfiguration.AddLogSource("cat1", SourceLevels.All, true, listener);

                this.writer = new LogWriter(loggingConfiguration);
                var logEntry = new LogEntry("message", "cat1", 0, 0, TraceEventType.Error, "title", null);
                this.writer.Write(logEntry);

                Assert.IsTrue(writer.ToString().IndexOf("\r\n  \"Message\": \"message\",\r\n  \"Categories\": [\r\n    \"cat1\"\r\n  ],\r\n  \"Priority\": 0,\r\n  \"EventId\": 0,\r\n  \"Severity\": 2,\r\n  \"LoggedSeverity\": \"Error\",\r\n  \"Title\": \"title\",\r\n  \"TimeStamp\": ") != -1);
            }
        }

        //// Check JsonFormatter-None in FormattedTextWriterTraceListener
        [TestMethod]
        public void MessageIsFormattedWithFormattingNoneWhenUsingJsonFormatterWithFormattedTextWriterTraceListener()
        {
            using (StringWriter writer = new StringWriter())
            {
                FormattedTextWriterTraceListener listener = new FormattedTextWriterTraceListener(writer, new JsonLogFormatter(JsonFormatting.None));

                LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
                loggingConfiguration.AddLogSource("cat1", SourceLevels.All, true, listener);

                this.writer = new LogWriter(loggingConfiguration);
                var logEntry = new LogEntry("message", "cat1", 0, 0, TraceEventType.Error, "title", null);
                this.writer.Write(logEntry);

                Assert.IsTrue(writer.ToString().IndexOf("\"Message\":\"message\",\"Categories\":[\"cat1\"],\"Priority\":0,\"EventId\":0,\"Severity\":2,\"LoggedSeverity\":\"Error\",\"Title\":\"title\",\"TimeStamp\":") != -1);
            }
        }

        // Check JsonFormatter-No formatting in FormattedTextWriterTraceListener
        [TestMethod]
        public void MessageIsFormattedWhenNoFormattingUsingJsonFormatterWithFormattedTextWriterTraceListener()
        {
            using (StringWriter writer = new StringWriter())
            {
                FormattedTextWriterTraceListener listener = new FormattedTextWriterTraceListener(writer, new JsonLogFormatter(JsonFormatting.None));

                LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
                loggingConfiguration.AddLogSource("cat1", SourceLevels.All, true, listener);

                this.writer = new LogWriter(loggingConfiguration);
                var logEntry = new LogEntry("message", "cat1", 0, 0, TraceEventType.Error, "title", null);
                this.writer.Write(logEntry);

                Assert.IsTrue(writer.ToString().IndexOf("\"Message\":\"message\",\"Categories\":[\"cat1\"],\"Priority\":0,\"EventId\":0,\"Severity\":2,\"LoggedSeverity\":\"Error\",\"Title\":\"title\",\"TimeStamp\":") != -1);
            }
        }

        //// Deserialize JsonFormatting.Indented
        [TestMethod]
        public void LogEntryIsDeserializedWhenUsingJsonFormatterIndented()
        {
            JsonLogFormatter jsonFormatter = new JsonLogFormatter(JsonFormatting.Indented);
            this.DeserializeLogEntry(jsonFormatter);
        }

        // Deserialize JsonFormatting.None
        [TestMethod]
        public void LogEntryIsDeserializedWhenUsingJsonFormatterNone()
        {
            JsonLogFormatter jsonFormatter = new JsonLogFormatter(JsonFormatting.None);
            this.DeserializeLogEntry(jsonFormatter);
        }

        // Deserialize with no JsonForamtting 
        [TestMethod]
        public void LogEntryIsDeserializedWhenUsingJsonFormatter()
        {
            JsonLogFormatter jsonFormatter = new JsonLogFormatter();
            this.DeserializeLogEntry(jsonFormatter);
        }

        private void DeserializeLogEntry(JsonLogFormatter jsonFormatter)
        {
            LogEntry entry = this.GetDefaultLogEntry();
            entry.Message = "message";
            entry.Title = "title";
            entry.Categories = new List<string>(new string[] { "cat1", "cat2", "cat3" });

            string serializedLogEntryText = jsonFormatter.Format(entry);
            LogEntry deserializedEntry = JsonLogFormatter.Deserialize<LogEntry>(serializedLogEntryText);

            Assert.IsNotNull(deserializedEntry);
            Assert.IsFalse(Object.ReferenceEquals(entry, deserializedEntry));
            Assert.AreEqual(entry.Categories.Count, deserializedEntry.Categories.Count);
            foreach (string category in entry.Categories)
            {
                Assert.IsTrue(deserializedEntry.Categories.Contains(category));
            }
            Assert.AreEqual(entry.Message, deserializedEntry.Message);
            Assert.AreEqual(entry.Title, deserializedEntry.Title);
        }
        private LogEntry GetDefaultLogEntry()
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
    }
}
