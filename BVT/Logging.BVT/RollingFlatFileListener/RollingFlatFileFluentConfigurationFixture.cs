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

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.RollingFlatFileListener
{
    [TestClass]
    public class RollingFlatFileFluentConfigurationFixture : LoggingFixtureBase
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
        public void EntryIsWrittenWhenLoggingWithBinaryFormatterUsingRollingFile()
        {
            const string FilePath = "rolling-sampleBinary.log";
            const string ListenerName = "Rolling File Listener";
            const string FormatterName = "Binary Formatter";
            const string Footer = "----------------";
            const string Header = "----------------";

            File.Delete(FilePath);

            configurationStart.WithOptions
                    .LogToCategoryNamed(CategoryName)
                        .WithOptions
                        .SetAsDefaultCategory()
                        .ToSourceLevels(SourceLevels.All)
                        .SendTo
                            .RollingFile(ListenerName)
                            .ToFile(FilePath)
                            .RollAfterSize(100)
                            .RollEvery(Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.RollInterval.Day)
                            .UseTimeStampPattern("ddmmyyyy")
                            .CleanUpArchivedFilesWhenMoreThan(5)
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

        [TestMethod]
        public void EntryIsWrittenWhenConfiguringLoggingWithRollingFile()
        {
            const string FilePath = "sample.log";
            const string ListenerName = "Rolling File Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string Footer = "Footer";
            const string Header = "Header";
            const int RollAfterSize = 150;
            const int NumberFiles = 5;
            const string TimeStampPattern = "yyyymmdd";
            RollInterval rollInterval = RollInterval.Day;
            RollFileExistsBehavior behavior = RollFileExistsBehavior.Increment;

            configurationStart.LogToCategoryNamed(CategoryName)
                            .WithOptions
                            .DoNotAutoFlushEntries()
                            .SetAsDefaultCategory()
                            .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .RollingFile(ListenerName)
                                        .RollEvery(rollInterval)
                                        .RollAfterSize(RollAfterSize)
                                        .CleanUpArchivedFilesWhenMoreThan(NumberFiles)
                                        .WhenRollFileExists(behavior)
                                        .UseTimeStampPattern(TimeStampPattern)
                                        .ToFile(FilePath)
                                            .WithFooter(Footer)
                                            .WithHeader(Header)
                                            .WithTraceOptions(TraceOptions.None)
                                                .FormatWith(new FormatterBuilder()
                                                .TextFormatterNamed(FormatterName)
                                                .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var rollingFile = settings.TraceListeners.Get(0) as RollingFlatFileTraceListenerData;

            Assert.IsNotNull(rollingFile);
            Assert.AreEqual(ListenerName, rollingFile.Name);
            Assert.AreEqual(behavior, rollingFile.RollFileExistsBehavior);
            Assert.AreEqual(rollInterval, rollingFile.RollInterval);
            Assert.AreEqual(RollAfterSize, rollingFile.RollSizeKB);
            Assert.AreEqual(TimeStampPattern, rollingFile.TimeStampPattern);
            Assert.AreEqual(NumberFiles, rollingFile.MaxArchivedFiles);
            Assert.AreEqual(FilePath, rollingFile.FileName);
            Assert.AreEqual(Footer, rollingFile.Footer);
            Assert.AreEqual(Header, rollingFile.Header);
            Assert.AreEqual(TraceOptions.None, rollingFile.TraceOutputOptions);
            Assert.AreEqual(FormatterName, rollingFile.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void EntryIsWrittenWhenConfiguringLoggingWithRollingFileWithDefaultValues()
        {
            const string DefaultFilePath = "rolling.log";
            const string ListenerName = "Rolling File Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string Footer = "Footer";
            const string Header = "Header";
            const int RollAfterSize = 150;
            const int NumberFiles = 5;
            const string TimeStampPattern = "yyyymmdd";
            RollInterval rollInterval = RollInterval.Day;
            RollFileExistsBehavior behavior = RollFileExistsBehavior.Increment;

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .RollingFile(ListenerName)
                                    .RollEvery(rollInterval)
                                    .RollAfterSize(RollAfterSize)
                                    .CleanUpArchivedFilesWhenMoreThan(NumberFiles)
                                    .WhenRollFileExists(behavior)
                                    .UseTimeStampPattern(TimeStampPattern)
                                    .WithFooter(Footer)
                                    .WithHeader(Header)
                                    .WithTraceOptions(TraceOptions.None)
                                        .FormatWith(new FormatterBuilder()
                                        .TextFormatterNamed(FormatterName)
                                        .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var rollingFile = settings.TraceListeners.Get(0) as RollingFlatFileTraceListenerData;

            Assert.IsNotNull(rollingFile);
            Assert.AreEqual(ListenerName, rollingFile.Name);
            Assert.AreEqual(behavior, rollingFile.RollFileExistsBehavior);
            Assert.AreEqual(rollInterval, rollingFile.RollInterval);
            Assert.AreEqual(RollAfterSize, rollingFile.RollSizeKB);
            Assert.AreEqual(TimeStampPattern, rollingFile.TimeStampPattern);
            Assert.AreEqual(NumberFiles, rollingFile.MaxArchivedFiles);
            Assert.AreEqual(DefaultFilePath, rollingFile.FileName);
            Assert.AreEqual(Footer, rollingFile.Footer);
            Assert.AreEqual(Header, rollingFile.Header);
            Assert.AreEqual(TraceOptions.None, rollingFile.TraceOutputOptions);
            Assert.AreEqual(FormatterName, rollingFile.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void EntryIsWrittenWhenLoggingUsingRollingFile()
        {
            const string FilePath = "rolling-sample.log";
            const string ListenerName = "Rolling File Listener";
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
                            .RollingFile(ListenerName)
                            .ToFile(FilePath)
                            .RollAfterSize(100)
                            .RollEvery(Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.RollInterval.Day)
                            .UseTimeStampPattern("ddmmyyyy")
                            .CleanUpArchivedFilesWhenMoreThan(5)
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
    }
}