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

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.FlatFileListener
{
    [TestClass]
    public class FlatFileFluentConfigurationFixture : LoggingFixtureBase
    {
        private new const string CategoryName = "Debug";

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
        public void EntryIsWrittenWhenLoggingUsingFlatFile()
        {
            const string FilePath = "sample.log";
            const string ListenerName = "Flat File Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string Footer = "Footer";
            const string Header = "Header";

            File.Delete(FilePath);

            configurationStart.WithOptions
                    .LogToCategoryNamed(CategoryName)
                        .WithOptions
                        .SetAsDefaultCategory()
                        .ToSourceLevels(SourceLevels.All)
                        .SendTo
                            .FlatFile(ListenerName)
                            .ToFile(FilePath)
                            .WithFooter(Footer)
                            .WithHeader(Header)
                            .WithTraceOptions(TraceOptions.None)
                            .FormatWith(new FormatterBuilder()
                            .TextFormatterNamed(FormatterName)
                            .UsingTemplate(Template));

            this.SetConfigurationSource();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(e));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            this.writer.Write(LogEntryFactory.GetLogEntry());

            FileInfo info = new FileInfo(FilePath);

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenLoggingWithBinaryFormatterUsingFlatFile()
        {
            const string FilePath = "binaryTemplateSample.log";
            const string ListenerName = "Flat File Listener";
            const string FormatterName = "Binary Formatter";
            const string Footer = "-----------------";
            const string Header = "-----------------";

            File.Delete(FilePath);

            configurationStart.WithOptions
                    .LogToCategoryNamed(CategoryName)
                        .WithOptions
                        .SetAsDefaultCategory()
                        .ToSourceLevels(SourceLevels.All)
                        .SendTo
                            .FlatFile(ListenerName)
                            .ToFile(FilePath)
                            .WithFooter(Footer)
                            .WithHeader(Header)
                            .WithTraceOptions(TraceOptions.None)
                            .FormatWith(new FormatterBuilder()
                            .BinaryFormatterNamed(FormatterName));

            this.SetConfigurationSource();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(e));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            this.writer.Write(LogEntryFactory.GetLogEntry());

            var deserializedEntry = LogFileReader.GetEntry(FilePath);

            Assert.AreEqual(LogEntryFactory.DefaultMessage, deserializedEntry.Message);
            Assert.AreEqual(2, deserializedEntry.Categories.Count);
            Assert.AreEqual(LogEntryFactory.DefaultEventId, deserializedEntry.EventId);
            Assert.AreEqual(LogEntryFactory.DefaultPriority, deserializedEntry.Priority);
        }
    }
}