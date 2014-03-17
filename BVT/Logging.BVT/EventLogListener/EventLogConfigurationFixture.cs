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

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.EventLogListener
{
    [TestClass]
    public class EventLogConfigurationFixture : LoggingFixtureBase
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

        //EventLog Listener Category Name Null
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenGeneralCategoryNameIsNull()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration1"));
        }

        //EventLog Listener Source Name Null
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenEventLogListenerIsSourceNameNull()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration2"));
            factory.Create();
        }

        //EventLog Listener Formatter Null
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenTextFormatterIsNull()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration3"));
        }

        [TestMethod]
        public void EntryIsWrittenWhenEventLogTraceListenerMin()
        {
            bool entrymade = false;

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration68"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();

            this.writer.Write(entry);

            EventLog events = new EventLog()
            {
                Log = "Application",
                Source = "Enterprise Library Logging"
            };

            foreach (EventLogEntry elogentry in events.Entries)
            {
                if (elogentry.Message.Contains("Message: Sample Message."))
                {
                    entrymade = true;
                    break;
                }
            }

            if (entrymade == false)
            {
                Assert.Fail("Eventlog message not logged");
            }
        }

        //EventLog Listener Name Null
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenEventLogListenerIsNameNull()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration1"));
        }

        //Event Log Listener Log fileName Null
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenLogNameNull()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration6"));
        }

        [TestMethod]
        public void EntryIsWrittenWhenUnprocessedCategoryEventLog()
        {
            var entry = LogEntryFactory.GetLogEntry();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration92"));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);
            this.writer.Write(entry);

            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Sample Message."));
        }

        [TestMethod]
        public void EntryIsWrittenWhenCategoriesSeverityAllEventLog()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration71"));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);
            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Sample Message."));
        }

        [TestMethod]
        public void EntryIsWrittenWhenSpecialCategoriesCriticalEventLogMax()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration76"));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);
            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Sample Message."));
        }

        [TestMethod]
        public void EntryIsWrittenWhenLogErrorsWarningsEventLog()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration93"));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Sample Message."));
        }
    }
}