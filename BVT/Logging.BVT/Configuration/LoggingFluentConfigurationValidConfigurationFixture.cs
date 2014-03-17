using System;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Database.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Configuration
{
    [TestClass]
    public class LoggingFluentConfigurationValidConfigurationFixture : LoggingFixtureBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            configurationStart.WithOptions
                    .DisableTracing()
                    .DoNotLogWarningsWhenNoCategoryExists()
                    .DoNotRevertImpersonation();
        }

        [TestMethod]
        public void SettingsAreCorrectWhenConfiguringLoggingStarts()
        {
            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.IsFalse(settings.LogWarningWhenNoCategoriesMatch);
            Assert.IsFalse(settings.TracingEnabled);
            Assert.IsFalse(settings.RevertImpersonation);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingUsingAllEventsSpecialSource()
        {
            const string AllEvents = "All Events";

            configurationStart.SpecialSources
                            .AllEventsCategory
                                .WithOptions
                                .ToSourceLevels(SourceLevels.All)
                                .DoNotAutoFlushEntries();

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(AllEvents, settings.SpecialTraceSources.AllEventsTraceSource.Name);
            Assert.IsFalse(settings.SpecialTraceSources.AllEventsTraceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.All, settings.SpecialTraceSources.AllEventsTraceSource.DefaultLevel);
            Assert.AreEqual(0, settings.SpecialTraceSources.AllEventsTraceSource.TraceListeners.Count);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingUsingAllEventsAsDefaultCategory()
        {
            const string AllEvents = "All Events";

            configurationStart.SpecialSources
                            .AllEventsCategory
                                .WithOptions
                                .ToSourceLevels(SourceLevels.All)
                                .DoNotAutoFlushEntries();

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(AllEvents, settings.SpecialTraceSources.AllEventsTraceSource.Name);
            Assert.IsFalse(settings.SpecialTraceSources.AllEventsTraceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.All, settings.SpecialTraceSources.AllEventsTraceSource.DefaultLevel);
            Assert.AreEqual(0, settings.SpecialTraceSources.AllEventsTraceSource.TraceListeners.Count);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingUsingLoggingErrorsAndWarningsAsDefaultCategory()
        {
            const string LoggingErrorsAndWarnings = "Logging Errors & Warnings";

            configurationStart.SpecialSources
                            .LoggingErrorsAndWarningsCategory
                                .WithOptions
                                .ToSourceLevels(SourceLevels.All)
                                .DoNotAutoFlushEntries();

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingErrorsAndWarnings, settings.SpecialTraceSources.ErrorsTraceSource.Name);
            Assert.IsFalse(settings.SpecialTraceSources.ErrorsTraceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.All, settings.SpecialTraceSources.ErrorsTraceSource.DefaultLevel);
            Assert.AreEqual(0, settings.SpecialTraceSources.ErrorsTraceSource.TraceListeners.Count);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingUsingUnprocessedAsDefaultCategory()
        {
            const string Unprocessed = "Unprocessed Category";

            configurationStart.SpecialSources
                            .UnprocessedCategory
                                .WithOptions
                                .ToSourceLevels(SourceLevels.All)
                                .DoNotAutoFlushEntries();

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(Unprocessed, settings.SpecialTraceSources.NotProcessedTraceSource.Name);
            Assert.IsFalse(settings.SpecialTraceSources.NotProcessedTraceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.All, settings.SpecialTraceSources.NotProcessedTraceSource.DefaultLevel);
            Assert.AreEqual(0, settings.SpecialTraceSources.NotProcessedTraceSource.TraceListeners.Count);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithFlatFile()
        {
            const string FilePath = "sample.log";
            const string ListenerName = "Flat File Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string Footer = "Footer";
            const string Header = "Header";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                    .DoNotAutoFlushEntries()
                                    .SetAsDefaultCategory()
                                    .ToSourceLevels(SourceLevels.ActivityTracing)
                                        .SendTo
                                            .FlatFile(ListenerName)
                                            .ToFile(FilePath)
                                                .WithFooter(Footer)
                                                .WithHeader(Header)
                                                .WithTraceOptions(TraceOptions.None)
                                                    .FormatWith(new FormatterBuilder()
                                                        .TextFormatterNamed(FormatterName)
                                                        .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var flatFile = settings.TraceListeners.Get(0) as FlatFileTraceListenerData;

            Assert.IsNotNull(flatFile);
            Assert.AreEqual(ListenerName, flatFile.Name);
            Assert.AreEqual(FilePath, flatFile.FileName);
            Assert.AreEqual(Footer, flatFile.Footer);
            Assert.AreEqual(Header, flatFile.Header);
            Assert.AreEqual(TraceOptions.None, flatFile.TraceOutputOptions);
            Assert.AreEqual(FormatterName, flatFile.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithFlatFileWithDefaultValues()
        {
            const string DefaultFilePath = "trace.log";
            const string ListenerName = "Flat File Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string Footer = "Footer";
            const string Header = "Header";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                    .SendTo
                                        .FlatFile(ListenerName)
                                        .WithFooter(Footer)
                                        .WithHeader(Header)
                                        .WithTraceOptions(TraceOptions.None)
                                            .FormatWith(new FormatterBuilder()
                                                .TextFormatterNamed(FormatterName)
                                                .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var flatFile = settings.TraceListeners.Get(0) as FlatFileTraceListenerData;

            Assert.IsNotNull(flatFile);
            Assert.AreEqual(ListenerName, flatFile.Name);
            Assert.AreEqual(DefaultFilePath, flatFile.FileName);
            Assert.AreEqual(Footer, flatFile.Footer);
            Assert.AreEqual(Header, flatFile.Header);
            Assert.AreEqual(TraceOptions.None, flatFile.TraceOutputOptions);
            Assert.AreEqual(FormatterName, flatFile.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithXmlFile()
        {
            const string FilePath = "sample.xml";
            const string ListenerName = "Xml File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                            .WithOptions
                            .DoNotAutoFlushEntries()
                            .SetAsDefaultCategory()
                            .ToSourceLevels(SourceLevels.ActivityTracing)
                            .SendTo
                                .XmlFile(ListenerName)
                                .ToFile(FilePath)
                                .WithTraceOptions(TraceOptions.None);

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var xmlFile = settings.TraceListeners.Get(0) as XmlTraceListenerData;

            Assert.IsNotNull(xmlFile);
            Assert.AreEqual(ListenerName, xmlFile.Name);
            Assert.AreEqual(FilePath, xmlFile.FileName);
            Assert.AreEqual(TraceOptions.None, xmlFile.TraceOutputOptions);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithXmlFileWithDefaultValues()
        {
            const string DefaultFilePath = "trace-xml.log";
            const string ListenerName = "Xml File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                    .SendTo
                                    .XmlFile(ListenerName)
                                    .WithTraceOptions(TraceOptions.None);

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var xmlFile = settings.TraceListeners.Get(0) as XmlTraceListenerData;

            Assert.IsNotNull(xmlFile);
            Assert.AreEqual(ListenerName, xmlFile.Name);
            Assert.AreEqual(DefaultFilePath, xmlFile.FileName);
            Assert.AreEqual(TraceOptions.None, xmlFile.TraceOutputOptions);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithCustomListener()
        {
            const string ListenerName = "Custom Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string InitData = "Init Data";
            
            NameValueCollection attributes = new NameValueCollection();

            attributes.Add("attr1", "1");
            attributes.Add("attr2", "2");

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .Custom(ListenerName, typeof(MyCustomTraceListener), attributes)
                                    .UsingInitData(InitData)
                                    .WithTraceOptions(TraceOptions.DateTime)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                    .TextFormatterNamed(FormatterName)
                                    .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var customListener = settings.TraceListeners.Get(0) as CustomTraceListenerData;

            Assert.IsNotNull(customListener);
            Assert.AreEqual(2, customListener.Attributes.Count);
            Assert.AreEqual("1", customListener.Attributes[0]);
            Assert.AreEqual("2", customListener.Attributes["attr2"]);
            Assert.AreEqual(ListenerName, customListener.Name);
            Assert.AreEqual(InitData, customListener.InitData);
            Assert.AreEqual(SourceLevels.Verbose, customListener.Filter);
            Assert.AreEqual(TraceOptions.DateTime, customListener.TraceOutputOptions);
            Assert.AreEqual(FormatterName, customListener.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithDatabase()
        {
            const string ListenerName = "Database Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string DatabaseName = "local";
            const string AddSProc = "add";
            const string WriteSProc = "write";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .Database(ListenerName)
                                    .UseDatabase(DatabaseName)
                                    .WithAddCategoryStoredProcedure(AddSProc)
                                    .WithWriteLogStoredProcedure(WriteSProc)
                                    .WithTraceOptions(TraceOptions.DateTime)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                    .TextFormatterNamed(FormatterName)
                                    .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var databaseListener = settings.TraceListeners.Get(0) as FormattedDatabaseTraceListenerData;

            Assert.IsNotNull(databaseListener);
            Assert.AreEqual(ListenerName, databaseListener.Name);
            Assert.AreEqual(DatabaseName, databaseListener.DatabaseInstanceName);
            Assert.AreEqual(WriteSProc, databaseListener.WriteLogStoredProcName);
            Assert.AreEqual(AddSProc, databaseListener.AddCategoryStoredProcName);
            Assert.AreEqual(SourceLevels.Verbose, databaseListener.Filter);
            Assert.AreEqual(TraceOptions.DateTime, databaseListener.TraceOutputOptions);
            Assert.AreEqual(FormatterName, databaseListener.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithDatabaseWithDefaultValues()
        {
            const string ListenerName = "Database Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string DatabaseName = "local";
            const string DefaultAddSProc = "AddCategory";
            const string DefaultWriteSProc = "WriteLog";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .Database(ListenerName)
                                    .UseDatabase(DatabaseName)
                                    .WithTraceOptions(TraceOptions.DateTime)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                        .TextFormatterNamed(FormatterName)
                                        .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var databaseListener = settings.TraceListeners.Get(0) as FormattedDatabaseTraceListenerData;

            Assert.IsNotNull(databaseListener);
            Assert.AreEqual(ListenerName, databaseListener.Name);
            Assert.AreEqual(DatabaseName, databaseListener.DatabaseInstanceName);
            Assert.AreEqual(DefaultWriteSProc, databaseListener.WriteLogStoredProcName);
            Assert.AreEqual(DefaultAddSProc, databaseListener.AddCategoryStoredProcName);
            Assert.AreEqual(SourceLevels.Verbose, databaseListener.Filter);
            Assert.AreEqual(TraceOptions.DateTime, databaseListener.TraceOutputOptions);
            Assert.AreEqual(FormatterName, databaseListener.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithEmail()
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

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var emailListener = settings.TraceListeners.Get(0) as EmailTraceListenerData;

            Assert.IsNotNull(emailListener);
            Assert.AreEqual(ListenerName, emailListener.Name);
            Assert.AreEqual(From, emailListener.FromAddress);
            Assert.AreEqual(SmtpServerPort, emailListener.SmtpPort);
            Assert.AreEqual(SmtpServer, emailListener.SmtpServer);
            Assert.AreEqual(SubjectEnd, emailListener.SubjectLineEnder);
            Assert.AreEqual(SubjectStart, emailListener.SubjectLineStarter);
            Assert.AreEqual(To, emailListener.ToAddress);
            Assert.AreEqual(SourceLevels.Verbose, emailListener.Filter);
            Assert.AreEqual(TraceOptions.DateTime, emailListener.TraceOutputOptions);
            Assert.AreEqual(FormatterName, emailListener.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);
            //Assert.AreEqual(typeof(CustomListener), diagnosticsListener.ListenerDataType);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithEmailWithDefaultValues()
        {
            const string ListenerName = "Email Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string From = "From";
            const string To = "To";
            const string DefaultSmtpServer = "127.0.0.1";
            const int DefaultSmtpServerPort = 25;
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
                                    .WithSubjectEnd(SubjectEnd)
                                    .WithSubjectStart(SubjectStart)
                                    .WithTraceOptions(TraceOptions.DateTime)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                        .TextFormatterNamed(FormatterName)
                                        .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var emailListener = settings.TraceListeners.Get(0) as EmailTraceListenerData;

            Assert.IsNotNull(emailListener);
            Assert.AreEqual(ListenerName, emailListener.Name);
            Assert.AreEqual(From, emailListener.FromAddress);
            Assert.AreEqual(DefaultSmtpServerPort, emailListener.SmtpPort);
            Assert.AreEqual(DefaultSmtpServer, emailListener.SmtpServer);
            Assert.AreEqual(SubjectEnd, emailListener.SubjectLineEnder);
            Assert.AreEqual(SubjectStart, emailListener.SubjectLineStarter);
            Assert.AreEqual(To, emailListener.ToAddress);
            Assert.AreEqual(SourceLevels.Verbose, emailListener.Filter);
            Assert.AreEqual(TraceOptions.DateTime, emailListener.TraceOutputOptions);
            Assert.AreEqual(FormatterName, emailListener.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);
            //Assert.AreEqual(typeof(CustomListener), diagnosticsListener.ListenerDataType);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithEventLog()
        {
            const string ListenerName = "Event Log Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string LogName = "LogName";
            const string MachineName = "MachineName";
            const string EventSource = "EventSource";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .EventLog(ListenerName)
                                    .ToLog(LogName)
                                    .ToMachine(MachineName)
                                    .UsingEventLogSource(EventSource)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                        .TextFormatterNamed(FormatterName)
                                        .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var eventLogListener = settings.TraceListeners.Get(0) as FormattedEventLogTraceListenerData;

            Assert.IsNotNull(eventLogListener);
            Assert.AreEqual(ListenerName, eventLogListener.Name);
            Assert.AreEqual(LogName, eventLogListener.Log);
            Assert.AreEqual(MachineName, eventLogListener.MachineName);
            Assert.AreEqual(EventSource, eventLogListener.Source);
            Assert.AreEqual(SourceLevels.Verbose, eventLogListener.Filter);
            Assert.AreEqual(TraceOptions.None, eventLogListener.TraceOutputOptions);
            Assert.AreEqual(FormatterName, eventLogListener.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithEventLogWithDefaultValues()
        {
            const string ListenerName = "Event Log Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string EventSource = "EventSource";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .EventLog(ListenerName)
                                    .UsingEventLogSource(EventSource)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                        .TextFormatterNamed(FormatterName)
                                        .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var eventLogListener = settings.TraceListeners.Get(0) as FormattedEventLogTraceListenerData;

            Assert.IsNotNull(eventLogListener);
            Assert.AreEqual(ListenerName, eventLogListener.Name);
            Assert.AreEqual(FormattedEventLogTraceListener.DefaultLogName, eventLogListener.Log);
            Assert.AreEqual(FormattedEventLogTraceListener.DefaultMachineName, eventLogListener.MachineName);
            Assert.AreEqual(EventSource, eventLogListener.Source);
            Assert.AreEqual(SourceLevels.Verbose, eventLogListener.Filter);
            Assert.AreEqual(TraceOptions.None, eventLogListener.TraceOutputOptions);
            Assert.AreEqual(FormatterName, eventLogListener.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithMsmq()
        {
            const string QueuePath = "QueuePath";
            const string ListenerName = "Msmq Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            System.Messaging.MessagePriority messagePriority = System.Messaging.MessagePriority.Lowest;
            TimeSpan timeToBeReceived = TimeSpan.MinValue;
            TimeSpan timeToReachQueue = TimeSpan.MaxValue;
            System.Messaging.MessageQueueTransactionType trxType = System.Messaging.MessageQueueTransactionType.Automatic;

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

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var msmqLogListener = settings.TraceListeners.Get(0) as MsmqTraceListenerData;

            Assert.IsNotNull(msmqLogListener);
            Assert.AreEqual(ListenerName, msmqLogListener.Name);
            Assert.AreEqual(messagePriority, msmqLogListener.MessagePriority);
            Assert.AreEqual(QueuePath, msmqLogListener.QueuePath);

            Assert.AreEqual(true, msmqLogListener.Recoverable);
            Assert.AreEqual(timeToBeReceived, msmqLogListener.TimeToBeReceived);
            Assert.AreEqual(timeToReachQueue, msmqLogListener.TimeToReachQueue);
            Assert.AreEqual(trxType, msmqLogListener.TransactionType);
            Assert.AreEqual(true, msmqLogListener.UseAuthentication);
            Assert.AreEqual(true, msmqLogListener.UseDeadLetterQueue);
            Assert.AreEqual(true, msmqLogListener.UseEncryption);
            Assert.AreEqual(SourceLevels.Verbose, msmqLogListener.Filter);
            Assert.AreEqual(TraceOptions.None, msmqLogListener.TraceOutputOptions);
            Assert.AreEqual(FormatterName, msmqLogListener.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithMsmqWithDefaultValues()
        {
            const string ListenerName = "Msmq Listener";
            const string FormatterName = "Text Formatter";
            const string Template = "My template";
            const string QueuePath = "QueuePath";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .Msmq(ListenerName)
                                    .UseQueue(QueuePath)
                                    .Filter(SourceLevels.Verbose)
                                    .FormatWith(new FormatterBuilder()
                                        .TextFormatterNamed(FormatterName)
                                        .UsingTemplate(Template));

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var msmqLogListener = settings.TraceListeners.Get(0) as MsmqTraceListenerData;

            Assert.IsNotNull(msmqLogListener);
            Assert.AreEqual(ListenerName, msmqLogListener.Name);
            Assert.AreEqual(MsmqTraceListenerData.DefaultPriority, msmqLogListener.MessagePriority);
            Assert.AreEqual(QueuePath, msmqLogListener.QueuePath);

            Assert.AreEqual(MsmqTraceListenerData.DefaultRecoverable, msmqLogListener.Recoverable);
            Assert.AreEqual(MsmqTraceListenerData.DefaultTimeToBeReceived, msmqLogListener.TimeToBeReceived);
            Assert.AreEqual(MsmqTraceListenerData.DefaultTimeToReachQueue, msmqLogListener.TimeToReachQueue);
            Assert.AreEqual(MsmqTraceListenerData.DefaultTransactionType, msmqLogListener.TransactionType);
            Assert.AreEqual(MsmqTraceListenerData.DefaultUseAuthentication, msmqLogListener.UseAuthentication);
            Assert.AreEqual(MsmqTraceListenerData.DefaultUseDeadLetter, msmqLogListener.UseDeadLetterQueue);
            Assert.AreEqual(MsmqTraceListenerData.DefaultUseEncryption, msmqLogListener.UseEncryption);
            Assert.AreEqual(SourceLevels.Verbose, msmqLogListener.Filter);
            Assert.AreEqual(TraceOptions.None, msmqLogListener.TraceOutputOptions);
            Assert.AreEqual(FormatterName, msmqLogListener.Formatter);
            Assert.AreEqual(1, settings.Formatters.Count);

            var formatter = settings.Formatters.Get(FormatterName) as TextFormatterData;

            Assert.IsNotNull(formatter);
            Assert.AreEqual(Template, formatter.Template);
        }

        [TestMethod]
        public void ConfigurationIsCorrectWhenConfiguringLoggingWithSystemDiagnostics()
        {
            const string ListenerName = "System Diagnostics Listener";
            const string InitData = "Init Data";

            configurationStart.LogToCategoryNamed(CategoryName)
                                .WithOptions
                                .DoNotAutoFlushEntries()
                                .SetAsDefaultCategory()
                                .ToSourceLevels(SourceLevels.ActivityTracing)
                                .SendTo
                                    .SystemDiagnosticsListener(ListenerName)
                                    .ForTraceListenerType(typeof(MyCustomTraceListener))
                                    .UsingInitData(InitData)
                                    .WithTraceOptions(TraceOptions.None)
                                    .Filter(SourceLevels.Verbose);

            LoggingSettings settings = GetLoggingSettings();

            Assert.IsNotNull(settings);

            Assert.AreEqual(LoggingFixtureBase.CategoryName, settings.DefaultCategory);
            Assert.AreEqual(1, settings.TraceSources.Count);

            var traceSource = settings.TraceSources.Get(0);

            Assert.IsFalse(traceSource.AutoFlush);
            Assert.AreEqual(SourceLevels.ActivityTracing, traceSource.DefaultLevel);
            Assert.AreEqual(ListenerName, traceSource.TraceListeners.Get(0).Name);
            Assert.AreEqual(1, settings.TraceListeners.Count);

            var diagnosticsListener = settings.TraceListeners.Get(0) as SystemDiagnosticsTraceListenerData;

            Assert.IsNotNull(diagnosticsListener);
            Assert.AreEqual(ListenerName, diagnosticsListener.Name);
            Assert.AreEqual(InitData, diagnosticsListener.InitData);
            //Assert.AreEqual(typeof(CustomListener), diagnosticsListener.ListenerDataType);
            Assert.AreEqual(SourceLevels.Verbose, diagnosticsListener.Filter);
            Assert.AreEqual(TraceOptions.None, diagnosticsListener.TraceOutputOptions);
            Assert.AreEqual(0, settings.Formatters.Count);
        }
    }
}