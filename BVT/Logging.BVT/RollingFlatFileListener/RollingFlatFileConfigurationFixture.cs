using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.RollingFlatFileListener
{
    [TestClass]
    public class RollingFlatFileConfigurationFixture : LoggingFixtureBase
    {
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

        private string GetTwoKBMessage()
        {
            return
                "1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-v1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890-12";
        }

        [TestMethod]
        public void ArchiveFilesExistWhenLoggingMultipleEntriesWithRollingFlatFileTraceListener()
        {
            if (Directory.Exists("RFFLogFiles"))
            {
                Directory.Delete("RFFLogFiles", true);
            }

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration99"));
            this.writer = factory.Create();

            for (int i = 0; i < 18; i++)
            {
                LogEntry entry = LogEntryFactory.GetLogEntry("RFFRoleOnSize", this.GetTwoKBMessage());

                this.writer.Write(entry);
            }

            this.writer.Dispose();

            Assert.AreEqual(5, Directory.GetFiles(".\\RFFLogFiles", "Log3*.log", SearchOption.TopDirectoryOnly).Length);

            Assert.IsTrue(File.Exists(Path.Combine("RFFLogFiles", String.Format("log3.{0}.14.log", DateTime.Now.Year))));
            Assert.IsTrue(File.Exists(Path.Combine("RFFLogFiles", String.Format("log3.{0}.15.log", DateTime.Now.Year))));
            Assert.IsTrue(File.Exists(Path.Combine("RFFLogFiles", String.Format("log3.{0}.16.log", DateTime.Now.Year))));
            Assert.IsTrue(File.Exists(Path.Combine("RFFLogFiles", String.Format("log3.{0}.17.log", DateTime.Now.Year))));
        }

        //Rolling flatfile File Name not provided
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenValidatingRffFileNameNotProvided()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration18"));
        }

        //Rolling flatfile File Name not provided
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenValidatingRffFormatterNotProvided()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration19"));
        }

        [TestMethod]
        public void EntryIsWrittenWhenRollingTraceListenerMin()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration65"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("rolling.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenRollingFFTraceListenerMax()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration67"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("rolling.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenCategoriesSeverityErrorRollingFlatFile()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration73"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("rolling.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenSpecialCategoriesVerboseRollingFlatFileMax()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration90"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("rolling.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenUnprocessedCategoryRollingFlatFile()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration96"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("rolling.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenLogErrorsWarningsRollingFlatFile()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration97"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo("rolling.log");

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }
    }
}