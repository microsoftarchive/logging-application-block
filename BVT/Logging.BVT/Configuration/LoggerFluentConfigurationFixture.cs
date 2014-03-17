using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Configuration
{
    [TestClass]
    public class LoggerFluentConfigurationFixture : LoggingFixtureBase
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

        [TestMethod]
        public void CustomListenerIsInvokedWhenConfigured()
        {
            const string ListenerName = "Custom Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.All)
                                .SendTo
                                    .Custom(ListenerName, typeof(MyCustomTraceListener))
                                    .WithTraceOptions(TraceOptions.DateTime)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                    .TextFormatterNamed(FormatterName)
                                    .UsingTemplate(Template));

            this.SetConfigurationSource();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(e));
            
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);
            LogEntry entry = LogEntryFactory.GetLogEntry();
            entry.Categories.Add(LoggingFixtureBase.CategoryName);

            this.writer.Write(entry);

            Assert.IsTrue(MyCustomTraceListener.Wrote);
        }

        [TestMethod]
        public void ResolveLoggerWithMsmqWhenConfigured()
        {
            const string ListenerName = "Msmq Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            System.Messaging.MessagePriority messagePriority = System.Messaging.MessagePriority.Lowest;
            TimeSpan timeToBeReceived = TimeSpan.MinValue;
            TimeSpan timeToReachQueue = TimeSpan.MaxValue;
            System.Messaging.MessageQueueTransactionType trxType = System.Messaging.MessageQueueTransactionType.Automatic;
            const string QueuePath = "QueuePath";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .Msmq(ListenerName)
                                    .UseQueue(QueuePath)
                                    .AsRecoverable()
                                    .Prioritize(messagePriority)
                                    .UseAuthentication()
                                    .UseDeadLetterQueue()
                                    .UseEncryption()
                                    .WithTransactionType(trxType)
                                    .SetTimeToBeReceived(timeToBeReceived)
                                    .SetTimeToReachQueue(timeToReachQueue)
                                    .WithTraceOptions(TraceOptions.None)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                        .TextFormatterNamed(FormatterName)
                                        .UsingTemplate(Template));

            this.SetConfigurationSource();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(e));

            this.writer = factory.Create();

            Assert.IsNotNull(this.writer);

            Assert.AreEqual(typeof(MsmqTraceListener), writer.TraceSources[CategoryName].Listeners.First().GetType());
        }

        [TestMethod]
        public void ResolveLoggerWithEmailWhenConfigured()
        {
            const string ListenerName = "Email Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string From = "From";
            const string To = "To";
            const string SmtpServer = "SmtpServer";
            const int SmtpServerPort = 125;
            const string SubjectEnd = "SubjectEnd";
            const string SubjectStart = "SubjectStart";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .Email(ListenerName)
                                    .From(From)
                                    .To(To)
                                    .UsingSmtpServer(SmtpServer)
                                    .UsingSmtpServerPort(SmtpServerPort)
                                    .WithSubjectEnd(SubjectEnd)
                                    .WithSubjectStart(SubjectStart)
                                    .WithTraceOptions(TraceOptions.DateTime)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                        .TextFormatterNamed(FormatterName)
                                        .UsingTemplate(Template));

            this.SetConfigurationSource();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(e));

            this.writer = factory.Create();

            Assert.IsNotNull(this.writer);

            Assert.AreEqual(typeof(EmailTraceListener), writer.TraceSources[CategoryName].Listeners.First().GetType());
        }
    }
}