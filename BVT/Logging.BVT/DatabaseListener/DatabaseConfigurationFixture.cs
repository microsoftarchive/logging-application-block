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
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.DatabaseListener
{
    [TestClass]
    public class DatabaseConfigurationFixture : LoggingFixtureBase
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
            DatabaseFactory.ClearDatabaseProviderFactory();
            base.TestInitialize();
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenAddCategoryEmpty()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration7"));
        }

        //Database TraceListener addCategoryStoredProcName Empty
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        [Ignore] // No exception thrown
        public void ExceptionIsThrownWhenAddCategoryStoredProcNameEmpty()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory(this.ConfigurationSource));
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration8"));
            factory.Create();
        }

        //Database TraceListener Name Empty
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenListenerNameEmpty()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration9"));
        }

        //Database TraceListener databaseInstanceName Empty
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        [Ignore] // No Exception thrown
        public void ExceptionIsThrownWhenDbInstanceNameEmpty()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory(this.ConfigurationSource));
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration10"));
            factory.Create();
        }

        //Database TraceListener writeLogStoredProcName Empty
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenWriteLogStoredProcNameEmpty()
        {
            DatabaseFactory.ClearDatabaseProviderFactory();
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory(this.ConfigurationSource));
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration11"));
            factory.Create();
        }

        [TestMethod]
        public void EntryIsLoggedWhenCategoryIsUnprocessed()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory(this.ConfigurationSource));

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration98"));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            LogEntry entry = new LogEntry();
            entry.Message = "Message 1";
            entry.Categories.Add("Most definitely UnprocessedCategory");
            entry.EventId = 123;
            entry.Priority = 11;
            entry.Severity = TraceEventType.Error;
            entry.Title = "Db Trace listener title1";

            this.writer.Write(entry);

            string result = GetLastLogMessage("Connection String");

            Assert.AreNotEqual(0, result.Length);
            Assert.AreEqual(entry.Message, result);
        }
    }
}
