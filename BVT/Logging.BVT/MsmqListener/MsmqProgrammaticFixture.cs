using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.MsmqListener
{
    [TestClass]
    public class MsmqProgrammaticFixture : LoggingFixtureBase
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

        //public void UpdateConfigForMSMQ(LoggingConfiguration loggingConfiguration)
        //{
        //    var MSMQTL = new MsmqTraceListener("TestMSMQProg", @".\Private$\entlib", briefFormatter,
        //        System.Messaging.MessagePriority.Normal, true, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0),
        //        false, true, false, System.Messaging.MessageQueueTransactionType.None);

        //    MSMQTL.Filter = new EventTypeFilter(SourceLevels.All);

        //    loggingConfiguration.AddLogSource("TestMSMQ", SourceLevels.All, true, MSMQTL);
        //    loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(MSMQTL);
        //}

        [TestMethod]
        public void EntryIsWrittenWhenMSMQGeneralCategory()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            var msmqListener = new MsmqTraceListener("TestMSMQProg", MsmqUtil.MessageQueuePath, new BinaryLogFormatter(),
                                      MessagePriority.Normal, false, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0),
                                      false, true, false, MessageQueueTransactionType.None);

            msmqListener.Filter = new EventTypeFilter(SourceLevels.All);

            MsmqUtil.ValidateMsmqIsRunning();
            MsmqUtil.DeletePrivateTestQ();
            MsmqUtil.CreatePrivateTestQ();

            LogSource clientSource = new LogSource("TestMSMQProg", new[] { msmqListener }, SourceLevels.All);
            LogSource distributorSource = new LogSource("TestMSMQProg", new[] { new MockTraceListener() }, SourceLevels.All);

            Dictionary<string, LogSource> traceSources = new Dictionary<string, LogSource>();
            this.writer = new LogWriter(new List<ILogFilter>(), traceSources, distributorSource, null, new LogSource("errors"), "General", false, false);
            Logger.SetLogWriter(this.writer);

            DistributorEventLogger eventLogger = new DistributorEventLogger();
            MsmqLogDistributor msmqDistributor = new MsmqLogDistributor(MsmqUtil.MessageQueuePath, eventLogger);
            msmqDistributor.StopReceiving = false;

            LogEntry logEntry = MsmqUtil.GetDefaultLogEntry();
            logEntry.Categories = new string[] { "MockCategoryOne" };
            logEntry.Message = MsmqUtil.MsgBody;
            logEntry.Severity = TraceEventType.Information;

            clientSource.TraceData(logEntry.Severity, 1, logEntry);

            msmqDistributor.CheckForMessages();

            Assert.AreEqual(1, MockTraceListener.Entries.Count);
            Assert.AreEqual(MsmqUtil.MsgBody, MockTraceListener.LastEntry.Message, "Body");
        }
    }
}