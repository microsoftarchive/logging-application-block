using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.FlatFileListener
{
    [TestClass]
    public class FlatFileProgrammaticFixture : LoggingFixtureBase
    {
        private const string BinaryFileName = "staticFacadeTrace.log";
        private const int DefaultPriority = -1;
        private const TraceEventType DefaultSeverity = TraceEventType.Information;
        private const int DefaultEventId = 1;
        private const string DefaultTitle = "";

        [TestInitialize]
        public override void TestInitialize()
        {
            this.strPath = @".\LogConfig" + Guid.NewGuid().ToString();
            LogFileReader.CreateDirectory(this.strPath);
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        private void UpdateFlatFileForConfig(LoggingConfiguration loggingConfiguration)
        {
            // Trace Listeners
            var flatFileTraceListener = new FlatFileTraceListener(Path.Combine(strPath, "FlatFile.log"), "----------------------------------------", "----------------------------------------", briefFormatter);
            flatFileTraceListener.Filter = new EventTypeFilter(SourceLevels.All);

            // Special Sources Configuration
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(flatFileTraceListener);
        }

        private void UpdateFlatFileForConfigNullParams(LoggingConfiguration loggingConfiguration)
        {
            // Trace Listeners
            var flatFileTraceListener = new FlatFileTraceListener(Path.Combine(strPath, "FlatFile.log"), null, null, null);
            flatFileTraceListener.Filter = new EventTypeFilter(SourceLevels.All);

            // Special Sources Configuration
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(flatFileTraceListener);
        }

        private void UpdateFlatFileForConfigStreamWriter(LoggingConfiguration loggingConfiguration)
        {
            // Trace Listeners
            StreamWriter writer = new StreamWriter(Path.Combine(strPath, "FlatFileStreamWriter.log"));
            FlatFileTraceListener flatFileTraceListener = new FlatFileTraceListener(writer);

            flatFileTraceListener.Filter = new EventTypeFilter(SourceLevels.All);

            // Special Sources Configuration
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(flatFileTraceListener);
        }

        private void UpdateFlatFileForConfigFileStream(LoggingConfiguration loggingConfiguration)
        {
            // Trace Listeners
            FileStream stream = new FileStream(Path.Combine(strPath, "FlatFileFileStream.log"), FileMode.OpenOrCreate);
            FlatFileTraceListener flatFileTraceListener = new FlatFileTraceListener(stream);

            //var flatFileTraceListener = new FlatFileTraceListener(Path.Combine(strPath, "FlatFile.log", "----------------------------------------", "----------------------------------------", briefFormatter);
            flatFileTraceListener.Filter = new EventTypeFilter(SourceLevels.All);

            // Special Sources Configuration
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(flatFileTraceListener);
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingMessage()
        {
            string originalFile = "message.log";

            File.Delete(originalFile);

            var logWriter = GetWriter(originalFile);

            string message = "Testing new overload";
            logWriter.Write(message);

            LogEntry deserializedEntry = LogFileReader.GetEntry(originalFile);

            Assert.AreEqual(message, deserializedEntry.Message);
            Assert.AreEqual(DefaultPriority, deserializedEntry.Priority);
            Assert.AreEqual(DefaultSeverity, deserializedEntry.Severity);
            Assert.AreEqual(DefaultTitle, deserializedEntry.Title);
            Assert.AreEqual(DefaultEventId, deserializedEntry.EventId);
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingMessageAndCategory()
        {
            string originalFile = "messageAndCategory.log";

            File.Delete(originalFile);

            var logWriter = GetWriter(originalFile);

            string message = "Testing new overload with category";
            logWriter.Write(message, "should not log");

            Assert.IsFalse(File.Exists(originalFile));
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingMessageCategoryAndPriority()
        {
            string originalFile = "messageCategoryPriority.log";

            File.Delete(originalFile);

            var logWriter = GetWriter(originalFile);

            string message = "Test message";
            logWriter.Write(message, "General", 3);

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
            string originalFile = "messageCategoryPriorityEventId.log";

            File.Delete(originalFile);

            var logWriter = GetWriter(originalFile);

            string message = "Test message";
            logWriter.Write(message, "General", 3, 2);

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
            string originalFile = "Severity.log";

            File.Delete(originalFile);

            var logWriter = GetWriter(originalFile);

            string message = "Test message";
            logWriter.Write(message, "General", 3, 2, TraceEventType.Resume);

            Assert.IsFalse(File.Exists(originalFile));
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingMessageCategoryPriorityEventIdSeverityAndTitle()
        {
            string originalFile = "catPriorityEventIdSeverityTitle.log";

            File.Delete(originalFile);

            var logWriter = GetWriter(originalFile);

            string message = "Test message";
            logWriter.Write(message, "General", 3, 2, TraceEventType.Critical, "My personal title");

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
            string originalFile = "ExtendedProperties.log";

            File.Delete(originalFile);

            var logWriter = GetWriter(originalFile);

            string message = "Test message";

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("one", 1);
            properties.Add("two", 2);
            properties.Add("three", 3);

            logWriter.Write(message, properties);

            LogEntry deserializedEntry = LogFileReader.GetEntry(originalFile);

            Assert.AreEqual(message, deserializedEntry.Message);
            Assert.AreEqual(3, deserializedEntry.ExtendedProperties.Count);
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingExtendedPropsAndCategory()
        {
            string originalFile = "ExtendedPropsCat.log";

            File.Delete(originalFile);

            var logWriter = GetWriter(originalFile);

            string message = "Test message";

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("one", 1);
            properties.Add("two", 2);
            properties.Add("three", 3);

            logWriter.Write(message, "do not log", properties);

            Assert.IsFalse(File.Exists(originalFile));
        }

        [TestMethod]
        public void EntryIsWrittenWhenWritingAnEntryPassingExtPropsCategoryAndPriority()
        {
            string originalFile = "ExtPropsCategoryAndPriority.log";

            File.Delete(originalFile);

            var logWriter = GetWriter(originalFile);

            string message = "Test message";

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("one", 1);
            properties.Add("two", 2);
            properties.Add("three", 3);

            logWriter.Write(message, "General", 3, properties);

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
            string originalFile = "ExtPropsCatPriorityEventIdSevTitle.log";

            File.Delete(originalFile);

            var logWriter = GetWriter(originalFile);

            string message = "Test message";

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("one", 1);
            properties.Add("two", 2);
            properties.Add("three", 3);

            logWriter.Write(message, "General", 3, 5, TraceEventType.Critical, "Sample Title", properties);

            LogEntry deserializedEntry = LogFileReader.GetEntry(originalFile);

            Assert.AreEqual(message, deserializedEntry.Message);
            Assert.AreEqual(3, deserializedEntry.Priority);
            Assert.AreEqual(TraceEventType.Critical, deserializedEntry.Severity);
            Assert.AreEqual("Sample Title", deserializedEntry.Title);
            Assert.AreEqual(5, deserializedEntry.EventId);
            Assert.AreEqual(3, deserializedEntry.ExtendedProperties.Count);
        }

        [TestMethod]
        public void EntryIsWrittenWhenAddingUnprocessedSpecialSource()
        {
            File.Delete("Trace.log");

            var config = new LoggingConfiguration();
            config.IsTracingEnabled = true;
            config.DefaultSource = "General";
            config.LogWarningsWhenNoCategoriesMatch = true;

            var flatFileTraceListener = new FlatFileTraceListener(@"Trace.log",
                "----------------------------------------",
                "----------------------------------------",
                new TextFormatter("Timestamp: {timestamp}{newline}&#xA;Message: {message}{newline}&#xA;Category: {category}{newline}&#xA;Priority: {priority}{newline}&#xA;EventId: {eventid}{newline}&#xA;Severity: {severity}{newline}&#xA;Title:{title}{newline}&#xA;Machine: {localMachine}{newline}&#xA;App Domain: {localAppDomain}{newline}&#xA;ProcessId: {localProcessId}{newline}&#xA;Process Name: {localProcessName}{newline}&#xA;Thread Name: {threadName}{newline}&#xA;Win32 ThreadId:{win32ThreadId}{newline}&#xA;Extended Properties: {dictionary({key} - {value}{newline})}"));

            config.SpecialSources.Unprocessed.Listeners.Add(flatFileTraceListener);

            this.writer = new LogWriter(config);
            this.writer.Write("Test Logging");
            this.writer.Dispose();

            Assert.IsTrue(File.Exists("Trace.log"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenFlatFileFileNameNull()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            FlatFileTraceListener flatFileTraceListener = new FlatFileTraceListener(null, "----------------------------------------", "----------------------------------------", briefFormatter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenFlatFileFilestreamNull()
        {
            FileStream stream = null;
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            FlatFileTraceListener flatFileTraceListener = new FlatFileTraceListener(stream);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenFlatFileStreamWriterNull()
        {
            StreamWriter writer = null;
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            FlatFileTraceListener flatFileTraceListener = new FlatFileTraceListener(writer);
        }

        [TestMethod]
        public void EntryIsWrittenWhenFlatFileGeneralCategory()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateFlatFileForConfig(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging");
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "FlatFile.log")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "FlatFile.log"));
            
            Assert.IsTrue(strFileData.Contains("Message: Test Logging"));
            Assert.IsTrue(strFileData.Contains("Category: General"));
        }

        [TestMethod]
        public void EntryIsWrittenWhenFlatFileGeneralCategoryStreamWriter()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateFlatFileForConfigStreamWriter(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging StreamWriter");
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "FlatFileStreamWriter.log")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "FlatFileStreamWriter.log"));
            
            Assert.IsTrue(strFileData.Contains("Message: Test Logging StreamWriter"));
            Assert.IsTrue(strFileData.Contains("Category: General"));
        }

        [TestMethod]
        public void EntryIsWrittenWhenFlatFileGeneralCategoryFileStream()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateFlatFileForConfigFileStream(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging Filestream");
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "FlatFileFileStream.log")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "FlatFileFileStream.log"));
            
            Assert.IsTrue(strFileData.Contains("Message: Test Logging Filestream"));
            Assert.IsTrue(strFileData.Contains("Category: General"));
        }

        [TestMethod]
        public void EntryIsWrittenWhenFlatFileUnprocessedCategory()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateFlatFileForConfig(loggingConfiguration);

            var unprocessedFlatFileTraceListener = new FlatFileTraceListener(
                Path.Combine(strPath, "Unprocessed.log"),
                "----------------------------------------",
                "----------------------------------------",
                null);

            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(unprocessedFlatFileTraceListener);
            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging");
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "FlatFile.log")));
            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "Unprocessed.log")));

            string strFileData1 = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "FlatFile.log"));
            
            Assert.IsTrue(strFileData1.Contains("Message: Test Logging"));
            Assert.IsTrue(strFileData1.Contains("Category: General"));

            string strFileData = LogFileReader.ReadFileWithoutLock((Path.Combine(this.strPath, "Unprocessed.log")));
            Assert.IsTrue(strFileData.Contains("Message: Test Logging"));
            Assert.IsTrue(strFileData.Contains("Unprocessed Category Information: 1"));
        }

        [TestMethod]
        public void EntryIsNotWrittenWhenFlatFileLogEnabledFilterIsFalse()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateFlatFileForConfig(loggingConfiguration);

            var logEnabledFilter = new LogEnabledFilter("LogEnabled Filter", false);
            loggingConfiguration.Filters.Add(logEnabledFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging Not Present");
            this.writer.Dispose();

            Assert.IsFalse(File.Exists(Path.Combine(this.strPath, "FlatFile.log")));
        }

        [TestMethod]
        public void OnlyEntriesInPriorityRangeAreWrittenWhenFlatFilePriorityFilter()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateFlatFileForConfig(loggingConfiguration);

            var priorityFilter = new PriorityFilter("Priority Filter", 2, 99);
            loggingConfiguration.Filters.Add(priorityFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging1", "General", 1);
            this.writer.Write("Test Logging2", "General", 2);
            this.writer.Write("Test Logging101", "General", 101);
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "FlatFile.log")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "FlatFile.log"));
            
            Assert.IsFalse(strFileData.Contains("Message: Test Logging1"));
            Assert.IsFalse(strFileData.Contains("Message: Test Logging101"));
            Assert.IsTrue(strFileData.Contains("Message: Test Logging2"));
        }

        [TestMethod]
        public void OnlyApplicableEntriesAreWrittenWhenFlatFileCategoryFilter()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateFlatFileForConfig(loggingConfiguration);

            // Category Filters
            ICollection<string> categories = new List<string>();
            categories.Add("BlockedByFilter");
            var categoryFilter = new CategoryFilter("Category Filter", categories, CategoryFilterMode.DenyAllExceptAllowed);
            loggingConfiguration.Filters.Add(categoryFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging1", "General", 1);
            this.writer.Write("Test Logging2", "BlockedByFilter", 2);
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "FlatFile.log")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "FlatFile.log"));
            
            Assert.IsFalse(strFileData.Contains("Message: Test Logging1"));
            Assert.IsTrue(strFileData.Contains("Message: Test Logging2"));
        }
    }
}