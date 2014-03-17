using System;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Extensibility;
using Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Configuration
{
    [TestClass]
    public class LoggingFluentConfigurationIncorrectUsageFixture : LoggingFixtureBase
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
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullCategoryName()
        {
            configurationStart.LogToCategoryNamed(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullFlatFileName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Ignore]
        [WorkItem(3680)]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullTimeStampPattern()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .RollingFile("rolling")
                .UseTimeStampPattern(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullRollingFileName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .RollingFile(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullFormatterName()
        {
            const string ListenerName = "Flat File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(ListenerName)
                .FormatWith(new FormatterBuilder()
                    .TextFormatterNamed(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullTemplate()
        {
            const string ListenerName = "Flat File Listener";
            const string FormatterName = "Text Formatter";

            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(ListenerName)
                .FormatWith(new FormatterBuilder()
                    .TextFormatterNamed(FormatterName)
                    .UsingTemplate(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullBinaryFormatter()
        {
            const string ListenerName = "Flat File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(ListenerName)
                .WithTraceOptions(TraceOptions.None)
                .FormatWith(new FormatterBuilder()
                    .BinaryFormatterNamed(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullGenericCustomFormatterName()
        {
            const string ListenerName = "Flat File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(ListenerName)
                .WithTraceOptions(TraceOptions.None)
                .FormatWith(new FormatterBuilder()
                    .CustomFormatterNamed<CustomFormatter>(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullGenericCustomFormatterAttributes()
        {
            const string ListenerName = "Flat File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(ListenerName)
                .WithTraceOptions(TraceOptions.None)
                .FormatWith(new FormatterBuilder()
                    .CustomFormatterNamed<CustomFormatter>("Custom", null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullCustomFormatterName()
        {
            const string ListenerName = "Flat File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(ListenerName)
                .WithTraceOptions(TraceOptions.None)
                .FormatWith(new FormatterBuilder()
                    .CustomFormatterNamed(null, typeof(CustomFormatter)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullCustomFormatter()
        {
            const string ListenerName = "Flat File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(ListenerName)
                .WithTraceOptions(TraceOptions.None)
                .FormatWith(new FormatterBuilder()
                    .CustomFormatterNamed("Custom Formatter", null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithCustomFormatterNotAssignableFromILogFormatter()
        {
            const string ListenerName = "Flat File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(ListenerName)
                .WithTraceOptions(TraceOptions.None)
                .FormatWith(new FormatterBuilder()
                    .CustomFormatterNamed("Custom Formatter", typeof(MarshalByRefObject)));
        }

        [TestMethod]
        [WorkItem(3696)]
        [Ignore]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullSharedFormatter()
        {
            const string ListenerName = "Flat File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(ListenerName)
                .FormatWithSharedFormatter(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullFileName()
        {
            const string ListenerName = "Flat File Listener";

            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .FlatFile(ListenerName)
                .ToFile(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullCustomTraceListenerName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Custom(null, typeof(CustomTraceListener));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullCustomTraceListener()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Custom("Custom", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithCustomTraceListenerNotAssignableFromCustomTraceListener()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Custom("Custom", typeof(MarshalByRefObject));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithInvalidGenericCustomTraceListenerName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Custom<CustomTraceListener>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullGenericCustomTraceListenerAttributes()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Custom<CustomTraceListener>("Custom", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullDatabaseName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Database(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Ignore]
        [WorkItem(3670)]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullUseDatabaseName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Database("Database").UseDatabase(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullAddCategoryStoredProcedure()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Database("Database")
                .WithAddCategoryStoredProcedure(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullWriteLogStoredProcedure()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Database("Database")
                .WithWriteLogStoredProcedure(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullEmailName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Email(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullTo()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Email("email")
                .To(null);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(3671)]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullSmtpServer()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Email("email")
                .UsingSmtpServer(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullEventLog()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .EventLog(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Ignore]
        [WorkItem(3762)]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullMachine()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .EventLog("Event Log")
                .ToMachine(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Ignore]
        [WorkItem(3763)]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullLogName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .EventLog("Event Log")
                .ToLog(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullEventLogSource()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .EventLog("Event Log")
                .UsingEventLogSource(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullMsmqName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Msmq(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Ignore]
        [WorkItem(3674)]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullQueue()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .Msmq("msmq")
                .UseQueue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullSharedListenerNamed()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .SharedListenerNamed(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullSystemDiagnosticsListenerName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .SystemDiagnosticsListener(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullSystemDiagnosticsTraceListenerType()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .SystemDiagnosticsListener("System")
                .ForTraceListenerType(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithSystemDiagnosticsTraceListenerTypeNotAssignableFromTraceListener()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .SystemDiagnosticsListener("System")
                .ForTraceListenerType(typeof(MarshalByRefObject));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Ignore]
        [WorkItem(3681)]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullInitData()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .SystemDiagnosticsListener("System")
                .UsingInitData(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullXmlFile()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .XmlFile(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenConfiguringLoggingWithNullXmlFileName()
        {
            configurationStart.LogToCategoryNamed(CategoryName)
                .WithOptions
                .DoNotAutoFlushEntries()
                .SetAsDefaultCategory()
                .ToSourceLevels(SourceLevels.ActivityTracing)
                .SendTo
                .XmlFile("xml")
                .ToFile(null);
        }
    }
}