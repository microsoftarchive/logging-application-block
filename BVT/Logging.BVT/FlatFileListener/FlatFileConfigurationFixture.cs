using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.FlatFileListener
{
    [TestClass]
    public class FlatFileConfigurationFixture : LoggingFixtureBase
    {
        private const string TraceFileName = "staticFacadeTraceOther.log";
        private const string BinaryFileName = "staticFacadeTrace.log";
        private const int DefaultPriority = -1;
        private const TraceEventType DefaultSeverity = TraceEventType.Information;
        private const int DefaultEventId = 1;
        private const string DefaultTitle = "";

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        private void ConfigureContainer(string fileName)
        {
            DictionaryConfigurationSource config = new DictionaryConfigurationSource();

            var logging = new LoggingSettings();
            logging.TracingEnabled = true;
            logging.Name = "Logging Application Block";
            logging.DefaultCategory = "General";
            logging.Formatters.Add(new BinaryLogFormatterData("Binary Formatter"));

            logging.TraceListeners.Add(
                new FlatFileTraceListenerData()
                {
                    Name = "TraceListener",
                    FileName = fileName,
                    TraceOutputOptions = TraceOptions.None,
                    Filter = SourceLevels.All,
                    Formatter = "Binary Formatter",
                    Type = typeof(FlatFileTraceListener)
                });

            var traceSource = new TraceSourceData("General", SourceLevels.Information, true);
            traceSource.TraceListeners.Add(new TraceListenerReferenceData("TraceListener"));

            logging.TraceSources.Add(traceSource);

            config.Add("loggingConfiguration101", logging);
            this.ConfigurationSource = config;
        }

        [TestMethod]
        public void EntryIsWrittenWhenLoggingUsingBinaryLogFormatter()
        {
            const string BinaryFileName = "trace.log";

            File.Delete(BinaryFileName);

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration102"));
            this.writer = factory.Create();

            LogEntry entry = LogEntryFactory.GetLogEntry();
            entry.Categories.Clear();
            entry.Categories.Add("General");

            if (writer.ShouldLog(entry))
            {
                this.writer.Write(entry);
            }

            Assert.IsTrue(File.Exists(BinaryFileName));

            FileInfo fileInfo = new FileInfo(BinaryFileName);

            Assert.IsTrue(fileInfo.Length > 0);

            LogEntry deserializedEntry = LogFileReader.GetEntry(BinaryFileName);

            Assert.AreEqual(entry.Message, deserializedEntry.Message);
            Assert.AreEqual(entry.Priority, deserializedEntry.Priority);
            Assert.AreEqual(entry.Severity, deserializedEntry.Severity);
        }

        //FlatFile TraceListener Formatter Not Provided
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenFlatFileFormatterNull()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration20"));
        }

        [TestMethod]
        public void EntryIsWrittenWhenCategoriesSeverityCriticalFlatFile()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration72"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("trace.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenSpecialCategoriesWarningFlatFileMax()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration83"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("trace.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenUnprocessedCategoryFlatFile()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration94"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("trace.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenLogErrorsWarningsFlatFile()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration95"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("trace.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        //FlatFile TraceListener FileName Null
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenFlatFileNameIsNull()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration17"));
            factory.Create();
        }

        //FlatFile TraceListener Null
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenFlatFileIsNull()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration16"));
            factory.Create();
        }

        [TestMethod]
        public void EntryIsWrittenWhenFlatFileTraceListenerMin()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration64"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("trace.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenFlatFileTraceListenerMax()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration66"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("trace.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenLoggingAnEntryPassingMessage()
        {
            const string OriginalFile = "messageLogger.log";

            File.Delete(OriginalFile);

            this.ConfigureContainer(OriginalFile);

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();

            string message = "Testing new overload";
            this.writer.Write(message);

            LogEntry deserializedEntry = LogFileReader.GetEntry(OriginalFile);

            Assert.AreEqual(message, deserializedEntry.Message);
            Assert.AreEqual(DefaultPriority, deserializedEntry.Priority);
            Assert.AreEqual(DefaultSeverity, deserializedEntry.Severity);
            Assert.AreEqual(DefaultTitle, deserializedEntry.Title);
            Assert.AreEqual(DefaultEventId, deserializedEntry.EventId);
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingMessageAndCategory()
        {
            string originalFile = "messageLoggerAndCategory.log";

            File.Delete(originalFile);

            this.ConfigureContainer(originalFile);
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();

            string message = "Testing new overload with category";
            this.writer.Write(message, "should not log");

            Assert.IsFalse(File.Exists(originalFile));
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingMessageCategoryAndPriority()
        {
            string originalFile = "messageLoggerCategoryPriority.log";

            File.Delete(originalFile);

            this.ConfigureContainer(originalFile);
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();

            string message = "Test message";
            this.writer.Write(message, "General", 3);

            LogEntry deserializedEntry = LogFileReader.GetEntry(originalFile);

            Assert.AreEqual(message, deserializedEntry.Message);
            Assert.AreEqual(3, deserializedEntry.Priority);
            Assert.AreEqual(DefaultSeverity, deserializedEntry.Severity);
            Assert.AreEqual(DefaultTitle, deserializedEntry.Title);
            Assert.AreEqual(DefaultEventId, deserializedEntry.EventId);
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingMessageCategoryPriorityAndEventId()
        {
            string originalFile = "messageLoggerCategoryPriorityEventId.log";

            File.Delete(originalFile);

            this.ConfigureContainer(originalFile);
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();

            string message = "Test message";
            this.writer.Write(message, "General", 3, 2);

            LogEntry deserializedEntry = LogFileReader.GetEntry(originalFile);

            Assert.AreEqual(message, deserializedEntry.Message);
            Assert.AreEqual(3, deserializedEntry.Priority);
            Assert.AreEqual(2, deserializedEntry.EventId);
            Assert.AreEqual(DefaultSeverity, deserializedEntry.Severity);
            Assert.AreEqual(DefaultTitle, deserializedEntry.Title);
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingMessageCategoryPriorityEventIdAndSeverity()
        {
            string originalFile = "SeverityLogger.log";

            File.Delete(originalFile);

            this.ConfigureContainer(originalFile);
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();

            string message = "Test message";
            this.writer.Write(message, "General", 3, 2, TraceEventType.Resume);

            Assert.IsFalse(File.Exists(originalFile));
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingMessageCategoryPriorityEventIdSeverityAndTitle()
        {
            string originalFile = "catLoggerPriorityEventIdSeverityTitle.log";

            File.Delete(originalFile);

            this.ConfigureContainer(originalFile);
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();

            string message = "Test message";
            this.writer.Write(message, "General", 3, 2, TraceEventType.Critical, "My personal title");

            LogEntry deserializedEntry = LogFileReader.GetEntry(originalFile);

            Assert.AreEqual(message, deserializedEntry.Message);
            Assert.AreEqual(3, deserializedEntry.Priority);
            Assert.AreEqual(2, deserializedEntry.EventId);
            Assert.AreEqual(TraceEventType.Critical, deserializedEntry.Severity);
            Assert.AreEqual("My personal title", deserializedEntry.Title);
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingExtendedProperties()
        {
            string originalFile = "LoggerExtendedProperties.log";

            File.Delete(originalFile);

            this.ConfigureContainer(originalFile);
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();

            string message = "Test message";

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("one", 1);
            properties.Add("two", 2);
            properties.Add("three", 3);

            this.writer.Write(message, properties);

            LogEntry deserializedEntry = LogFileReader.GetEntry(originalFile);

            Assert.AreEqual(message, deserializedEntry.Message);

            Assert.AreEqual(3, deserializedEntry.ExtendedProperties.Count);
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingExtendedPropsAndCategory()
        {
            string originalFile = "LoggerExtendedPropsCat.log";

            this.ConfigureContainer(originalFile);
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();

            string message = "Test message";

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("one", 1);
            properties.Add("two", 2);
            properties.Add("three", 3);

            this.writer.Write(message, "do not log", properties);

            Assert.IsFalse(File.Exists(originalFile));
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingExtPropsCategoryAndPriority()
        {
            string originalFile = "LoggerExtPropsCategoryAndPriority.log";

            File.Delete(originalFile);

            this.ConfigureContainer(originalFile);
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();

            string message = "Test message";

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("one", 1);
            properties.Add("two", 2);
            properties.Add("three", 3);

            this.writer.Write(message, "General", 3, properties);

            LogEntry deserializedEntry = LogFileReader.GetEntry(originalFile);

            Assert.AreEqual(message, deserializedEntry.Message);
            Assert.AreEqual(3, deserializedEntry.Priority);
            Assert.AreEqual(DefaultSeverity, deserializedEntry.Severity);
            Assert.AreEqual(DefaultTitle, deserializedEntry.Title);
            Assert.AreEqual(DefaultEventId, deserializedEntry.EventId);
            Assert.AreEqual(3, deserializedEntry.ExtendedProperties.Count);
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingExtPropsCategoryPriorityEventIdSeverityAndTitle()
        {
            string originalFile = "LoggerExtPropsCatPriorityEventIdSevTitle.log";

            this.ConfigureContainer(originalFile);
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();

            string message = "Test message";

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("one", 1);
            properties.Add("two", 2);
            properties.Add("three", 3);

            this.writer.Write(message, "General", 3, 5, TraceEventType.Information, "Sample Title", properties);

            LogEntry deserializedEntry = LogFileReader.GetEntry(originalFile);

            Assert.AreEqual(message, deserializedEntry.Message);
            Assert.AreEqual(3, deserializedEntry.Priority);
            Assert.AreEqual(TraceEventType.Information, deserializedEntry.Severity);
            Assert.AreEqual("Sample Title", deserializedEntry.Title);
            Assert.AreEqual(5, deserializedEntry.EventId);
            Assert.AreEqual(3, deserializedEntry.ExtendedProperties.Count);
        }

        [TestMethod]
        public void EntryIsWrittenWhenLoggerWritesALogEntry()
        {
            File.Delete(TraceFileName);

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            LogEntry entry = LogEntryFactory.GetLogEntry();
            entry.Categories.Clear();
            entry.Categories.Add("General");

            Logger.Write(entry);

            Assert.IsTrue(File.Exists(TraceFileName));

            FileInfo fileInfo = new FileInfo(TraceFileName);

            Assert.IsTrue(fileInfo.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenLoggingUsingBinaryFormatter()
        {
            File.Delete(BinaryFileName);

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            LogEntry entry = LogEntryFactory.GetLogEntry();

            entry.Categories.Clear();
            entry.Categories.Add("General");

            this.writer.Write(entry);

            Assert.IsTrue(File.Exists(BinaryFileName));

            FileInfo fileInfo = new FileInfo(BinaryFileName);

            Assert.IsTrue(fileInfo.Length > 0);

            LogEntry deserializedEntry = LogFileReader.GetEntry(BinaryFileName);

            Assert.AreEqual(entry.Message, deserializedEntry.Message);
            Assert.AreEqual(entry.Priority, deserializedEntry.Priority);
            Assert.AreEqual(entry.Severity, deserializedEntry.Severity);
        }
    }
}