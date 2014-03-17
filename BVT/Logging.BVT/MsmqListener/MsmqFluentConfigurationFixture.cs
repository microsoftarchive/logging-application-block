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
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.MsmqListener
{
    [TestClass]
    public class MsmqFluentConfigurationFixture : LoggingFixtureBase
    {
        private const string ListenerName = "Msmq Listener";
        private const string FormatterName = "Text Formatter";
        private const string Template = "My template";
        private const MessagePriority MessagePriority = System.Messaging.MessagePriority.Lowest;
        private const string QueuePath = MsmqUtil.MessageQueuePath;

        [TestInitialize]
        public override void TestInitialize()
        {
            MsmqUtil.ValidateMsmqIsRunning();
            MsmqUtil.DeletePrivateTestQ();
            MsmqUtil.CreatePrivateTestQ();

            base.TestInitialize();
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        [TestMethod]
        public void EntryIsWrittenWhenNoTransaction()
        {
            TimeSpan timeToBeReceived = TimeSpan.FromDays(1);
            TimeSpan timeToReachQueue = TimeSpan.Parse("49710.06:28:15");
            System.Messaging.MessageQueueTransactionType trxType = System.Messaging.MessageQueueTransactionType.None;

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.All)
                                .SendTo
                                    .Msmq(ListenerName)
                                    .UseQueue(QueuePath)
                                    .AsRecoverable()
                                    .Prioritize(MessagePriority)
                                    .UseDeadLetterQueue()
                                    .WithTransactionType(trxType)
                                    .SetTimeToBeReceived(timeToBeReceived)
                                    .SetTimeToReachQueue(timeToReachQueue)
                                    .WithTraceOptions(TraceOptions.None)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                        .TextFormatterNamed(FormatterName)
                                        .UsingTemplate(Template));

            this.SetConfigurationSource();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(LoggingSettings.SectionName));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            LogEntry entry = LogEntryFactory.GetLogEntry(CategoryName, "Test Message");
            this.writer.Write(entry);

            string entryText = MsmqUtil.GetLogEntryFromQueue();

            Assert.IsTrue(entryText == "<?xml version=\"1.0\"?>\r\n<string>My template</string>");
        }

        [TestMethod]
        public void EntryIsNotWrittenWhenTransactionalQueueAndNoTransaction()
        {
            MsmqUtil.ValidateMsmqIsRunning();
            MsmqUtil.DeletePrivateTestQ();
            MsmqUtil.CreatePrivateTestQ();

            System.Messaging.MessagePriority messagePriority = System.Messaging.MessagePriority.Lowest;
            System.Messaging.MessageQueueTransactionType trxType = System.Messaging.MessageQueueTransactionType.Single;
            const string QueuePath = MsmqUtil.MessageQueuePath;

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.All)
                                .SendTo
                                    .Msmq(ListenerName)
                                    .UseQueue(QueuePath)
                                    .AsRecoverable()
                                    .Prioritize(messagePriority)
                                    .UseDeadLetterQueue()
                                    .WithTransactionType(trxType)
                                    .WithTraceOptions(TraceOptions.None)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                        .TextFormatterNamed(FormatterName)
                                        .UsingTemplate(Template));

            this.SetConfigurationSource();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(LoggingSettings.SectionName));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            LogEntry entry = LogEntryFactory.GetLogEntry(CategoryName, "Test Message");
            this.writer.Write(entry);

            Assert.IsNull(MsmqUtil.GetLogEntryFromQueue());
        }
    }
}