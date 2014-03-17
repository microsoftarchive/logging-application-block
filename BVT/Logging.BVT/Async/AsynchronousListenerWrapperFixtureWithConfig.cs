using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Logging.Database;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Async
{
    [TestClass]
    public class AsynchronousListenerWrapperFixtureWithConfig : LoggingFixtureBase
    {
        public AsynchronousListenerWrapperFixtureWithConfig()
            : base("Async.AsyncLogging.config")
        { }

        [TestInitialize]
        public override void TestInitialize()
        {
            Logger.Reset();
            base.TestInitialize();
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        [TestMethod]
        public void AllEntriesAreWrittenWhenUsingAsynchForFlatFile()
        {
            string fileName = "AsynchTestingConfigForFlatFile.log";
            File.Delete(fileName);

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration2"));

            using (var writer = factory.Create())
            {
                Parallel.Invoke(Enumerable.Range(0, 2000).Select(i =>
                new Action(() =>
                {
                    writer.Write("Test Asynch Message Config: " + i);
                })).ToArray());
            }

            Assert.IsTrue(File.Exists(fileName));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(fileName).Contains("Test Asynch Message Config: 0"));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(fileName).Contains("Test Asynch Message Config: 1999"));
        }

        [TestMethod]
        public void MultipleFilesAreCreatedWhenUsingAsynchForRollingFlatFile()
        {
            string fileNameWithoutExtension = "AsynchTestingConfigForRollingFlatFile";
            string extension = ".log";
            string fileName = fileNameWithoutExtension + extension;
            var files = Directory.GetFiles(".", "AsynchTestingConfigForRollingFlatFile.*");

            foreach (var file in files)
            {
                File.Delete(file);
            }

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration3"));

            using (var writer = factory.Create())
            {
                Parallel.Invoke(Enumerable.Range(0, 150).Select(i =>
                new Action(() =>
                {
                    writer.Write("Test Asynch Message Config: " + i);
                })).ToArray());
            }

            Assert.IsTrue(File.Exists(fileName));
            Assert.IsTrue(File.Exists(fileNameWithoutExtension + "." + DateTime.Now.Year + ".1" + extension));
            Assert.IsTrue(File.Exists(fileNameWithoutExtension + "." + DateTime.Now.Year + ".6" + extension));
        }

        [TestMethod]
        public void AllEntriesAreWrittenWhenAsynchForFormattedEventLog()
        {
            string unique = Guid.NewGuid().ToString();
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration1"));

            using (var writer = factory.Create())
            {
                Parallel.Invoke(Enumerable.Range(0, 10).Select(i =>
                new Action(() =>
                {
                    writer.Write(unique + " Test Asynch Message Config: " + i);
                })).ToArray());
            }

            Assert.IsTrue(this.CheckForEntryInEventlog(unique + " Test Asynch Message Config: 0"));
            Assert.IsTrue(this.CheckForEntryInEventlog(unique + " Test Asynch Message Config: 9"));
        }

        [TestMethod]
        public void AllEntriesAreWrittenWhenUsingAsynchForXML()
        {
            string xmlFileName = "XMLFile.xml";
            File.Delete(xmlFileName);

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration4"));

            using (var writer = factory.Create())
            {
                Parallel.Invoke(Enumerable.Range(0, 200).Select(i =>
                new Action(() =>
                {
                    writer.Write("Test Asynch Message Config: " + i);
                })).ToArray());
            }

            Assert.IsTrue(File.Exists(xmlFileName));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(xmlFileName).Contains("Test Asynch Message Config: 0"));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(xmlFileName).Contains("Test Asynch Message Config: 199"));
        }

        [TestMethod]
        public void AllEntriesAreWrittenWhenUsingAsynchForXMLWithDisposeTimeoutInfinite()
        {
            string xmlFileName = "XMLFileInfinite.xml";
            File.Delete(xmlFileName);

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration5"));

            using (var writer = factory.Create())
            {
                Parallel.Invoke(Enumerable.Range(0, 10000).Select(i =>
                new Action(() =>
                {
                    writer.Write("Test Asynch Message Config: " + i);
                })).ToArray());
            }

            Assert.IsTrue(File.Exists(xmlFileName));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(xmlFileName).Contains("Test Asynch Message Config: 0"));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(xmlFileName).Contains("Test Asynch Message Config: 9999"));
        }

        [TestMethod]
        public void AllEntriesAreWrittenWhenUsingAsynchForXMLWithDisposeTimeoutShortTimespan()
        {
            string xmlFileName = "XMLFileShortTimespan.xml";
            File.Delete(xmlFileName);

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration6"));

            using (var writer = factory.Create())
            {
                Parallel.Invoke(Enumerable.Range(0, 10000).Select(i =>
                new Action(() =>
                {
                    writer.Write("Test Asynch Message Config: " + i);
                })).ToArray());
            }

            Assert.IsTrue(File.Exists(xmlFileName));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(xmlFileName).Contains("Test Asynch Message Config: 0"));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(xmlFileName).Contains("Test Asynch Message Config: 9999"));
        }

        [TestMethod]
        public void AllEntriesAreWrittenWhenUsingAsynchForFormattedDatabase()
        {
            this.ClearLogs();
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory(this.ConfigurationSource.GetSection), false);
            DatabaseProviderFactory factory = new DatabaseProviderFactory(this.ConfigurationSource.GetSection);
            Data.Database dbProvider = factory.CreateDefault();

            FormattedDatabaseTraceListener listener = new FormattedDatabaseTraceListener(dbProvider, "WriteLog", "AddCategory", new TextFormatter("TEST{newline}TEST"));
            string unique = Guid.NewGuid().ToString();
            LogWriterFactory factory1 = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration"));

            int messageContents = 0;
            using (var writer = factory1.Create())
            {
                Logger.SetLogWriter(writer);

                Parallel.Invoke(Enumerable.Range(0, 10000).Select(i =>
                new Action(() =>
                {
                    Logger.Write(unique + " Test Asynch Message: " + i);
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