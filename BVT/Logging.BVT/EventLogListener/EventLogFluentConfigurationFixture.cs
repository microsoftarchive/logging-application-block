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
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.EventLogListener
{
    [TestClass]
    public class EventLogFluentConfigurationFixture : LoggingFixtureBase
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
        public void EntryIsWrittenWhenLoggingUsingEventLog()
        {
            const string ListenerName = "Event Log Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";

            configurationStart.WithOptions
                    .LogToCategoryNamed(CategoryName)
                        .WithOptions
                        .SetAsDefaultCategory()
                        .ToSourceLevels(SourceLevels.All)
                        .SendTo
                            .EventLog(ListenerName)
                            .WithTraceOptions(TraceOptions.None)
                            .FormatWith(new FormatterBuilder()
                            .TextFormatterNamed(FormatterName)
                            .UsingTemplate(Template));

            this.SetConfigurationSource();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(e));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            LogEntry logEntry = LogEntryFactory.GetLogEntry();
            this.writer.Write(logEntry);

            Assert.IsTrue(this.CheckForEntryInEventlog(Template));
        }
    }
}
