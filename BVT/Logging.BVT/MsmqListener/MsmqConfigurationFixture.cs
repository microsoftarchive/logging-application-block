using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.MsmqListener
{
    [TestClass]
    public class MsmqConfigurationFixture : LoggingFixtureBase
    {
        private const string ErrorMessage = "Expected exception was not thrown.";

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

        [TestMethod]
        public void EntryIsWrittenWhenMinMSMQTraceListener()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration57"));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);
        }

        [TestMethod]
        public void EntryIsWrittenWhenMaxMSMQTraceListener()
        {
            MsmqUtil.ValidateMsmqIsRunning();
            MsmqUtil.DeletePrivateTestQ();
            MsmqUtil.CreatePrivateTestQ();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration63"));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            entry.Categories.Add("General");

            this.writer.Write(entry);
            this.writer.Dispose();

            string entryText = MsmqUtil.GetLogEntryFromQueue();
            Assert.IsTrue(entryText.Contains(entry.Message));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenEmptyNameMSMQTraceListener()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration58"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExceptionIsThrownWhenEmptyFormatterMSMQTraceListener()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration59"));
            factory.Create();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExceptionIsThrownWhenEmptyQueuePathMSMQTraceListener()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration60"));
            factory.Create();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenEmptyTimeToReachQueueMSMQTraceListener()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration61"));
            factory.Create();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenEmptyTimeToBeReceivedMSMQTraceListener()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration62"));
            factory.Create();
        }
    }
}