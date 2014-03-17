using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Logging.Database;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Async
{
    [TestClass]
    public class AsynchronousListenerWrapperFixture : LoggingFixtureBase
    {
        [TestMethod]
        public void AllEntriesAreWrittenWhenUsingAsynchForFlatFile()
        {
            string fileName = "AsynchTestingForFlatFile.log";
            File.Delete(fileName);

            var configuration = new LoggingConfiguration();
            configuration.AddLogSource("TestAsynchLogging In FlatFile").AddTraceListener(new AsynchronousTraceListenerWrapper(
                new FlatFileTraceListener(fileName), ownsWrappedTraceListener: true, bufferSize: 10000, disposeTimeout: TimeSpan.FromSeconds(5)));

            using (var writer = new LogWriter(configuration))
            {
                Parallel.Invoke(Enumerable.Range(0, 10000).Select(i =>
                new Action(() =>
                {
                    writer.Write("Test Asynch Message: " + i);
                })).ToArray());
            }
            
            Assert.IsTrue(File.Exists(fileName));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(fileName).Contains("Test Asynch Message: 0"));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(fileName).Contains("Test Asynch Message: 9999"));
        }

        [TestMethod]
        public void MultipleFilesAreWrittenWhenUsingAsynchForRollingFlatFile()
        {
            const string Folder = "AsyncFolder";
            const string FileNameWithoutExtension = "AsynchTestingForRollingFlatFile";
            const string Extension = ".log";
            const string FileName = FileNameWithoutExtension + Extension;
            
            LogFileReader.CreateDirectory(Folder);

            var configuration = new LoggingConfiguration();
            configuration.AddLogSource("TestAsynchLogging In RollingFlatFile").AddTraceListener(new AsynchronousTraceListenerWrapper(
                new RollingFlatFileTraceListener(Path.Combine(Folder, FileName), "----", "----", new TextFormatter(), 8, "yyyy",
                    RollFileExistsBehavior.Increment, RollInterval.Minute, 10), ownsWrappedTraceListener: true, bufferSize: 100));

            using (var writer = new LogWriter(configuration))
            {
                Parallel.Invoke(Enumerable.Range(0, 100).Select(i =>
                new Action(() =>
                {
                    writer.Write("Test Asynch Message: " + i);
                })).ToArray());
            }

            Assert.IsTrue(File.Exists(Path.Combine(Folder, FileName)));
            Assert.IsTrue(File.Exists(Path.Combine(Folder, FileNameWithoutExtension + "." + DateTime.Now.Year + ".1" + Extension)));
            Assert.IsTrue(File.Exists(Path.Combine(Folder, FileNameWithoutExtension + "." + DateTime.Now.Year + ".5" + Extension)));
        }

        [TestMethod]
        public void MultipleEntriesAreWrittenWhenUsingAsynchForFormattedEventLog()
        {
            string unique = Guid.NewGuid().ToString();
            var eventLog = new EventLog("Application", ".", "Enterprise Library Logging");
            var configuration = new LoggingConfiguration();
            configuration.AddLogSource("TestAsynchLogging In FormattedEventLog").AddTraceListener(new AsynchronousTraceListenerWrapper(
                new FormattedEventLogTraceListener(eventLog), ownsWrappedTraceListener: true, bufferSize: 10));

            using (var writer = new LogWriter(configuration))
            {
                Parallel.Invoke(Enumerable.Range(0, 10).Select(i =>
                new Action(() =>
                {
                    writer.Write(unique + " Test Asynch Message: " + i);
                })).ToArray());
            }

            Assert.IsTrue(this.CheckForEntryInEventlog(unique + " Test Asynch Message: 0"));
            Assert.IsTrue(this.CheckForEntryInEventlog(unique + " Test Asynch Message: 9"));
        }

        [TestMethod]
        public void MultipleEntriesAreWrittenWhenUsingAsynchForXML()
        {
            string xmlFileName = "XMLFile.xml";
            File.Delete(xmlFileName);

            var configuration = new LoggingConfiguration();
            configuration.AddLogSource("TestAsynchLogging In XML file").AddTraceListener(new AsynchronousTraceListenerWrapper(
                new XmlTraceListener(xmlFileName), ownsWrappedTraceListener: true, bufferSize: 100));

            using (var writer = new LogWriter(configuration))
            {
                Parallel.Invoke(Enumerable.Range(0, 200).Select(i =>
                new Action(() =>
                {
                    writer.Write("Test Asynch Message: " + i);
                })).ToArray());
            }

            Assert.IsTrue(File.Exists(xmlFileName));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(xmlFileName).Contains("Test Asynch Message: 0"));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(xmlFileName).Contains("Test Asynch Message: 99"));
        }

        [TestMethod]
        public void AllEntriesAreWrittenWhenUsingAsynchForFormattedDatabase()
        {
            this.ClearLogs();
            string unique = Guid.NewGuid().ToString();
            SqlDatabase dbConnection = new SqlDatabase(connectionString);
            int messageContents = 0;

            var configuration = new LoggingConfiguration();
            configuration.AddLogSource("TestAsynchLogging In FormattedDatabase")
                         .AddTraceListener(new AsynchronousTraceListenerWrapper(
                            new FormattedDatabaseTraceListener(dbConnection, "WriteLog", "AddCategory", null),
                            ownsWrappedTraceListener: true,
                            bufferSize: 10000));

            using (var writer = new LogWriter(configuration))
            {
                Parallel.Invoke(Enumerable.Range(0, 10000).Select(i =>
                new Action(() =>
                {
                    writer.Write(unique + " Test Asynch Message: " + i);
                })).ToArray());
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select COUNT(1) from Log where Message like '" + unique + " Test Asynch Message:%'", connection);
                messageContents = (Int32)command.ExecuteScalar();
                connection.Close();
            }

            Assert.AreEqual(10000, messageContents);
        }
    }
}