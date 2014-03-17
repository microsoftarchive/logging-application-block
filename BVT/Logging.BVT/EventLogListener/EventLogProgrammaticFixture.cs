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
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.EventLogListener
{
    [TestClass]
    public class EventLogProgrammaticFixture : LoggingFixtureBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        private void UpdateConfigForEventlog(LoggingConfiguration loggingConfiguration)
        {
            var eventLog = new EventLog("Application", ".", "Enterprise Library Logging");
            var eventLogTraceListener = new FormattedEventLogTraceListener(eventLog);
            loggingConfiguration.AddLogSource("General", SourceLevels.All, true, eventLogTraceListener);
            loggingConfiguration.SpecialSources.LoggingErrorsAndWarnings.Listeners.Add(eventLogTraceListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(eventLogTraceListener);
        }

        private void UpdateConfigForEventlogWithFormatter(LoggingConfiguration loggingConfiguration)
        {
            var eventLog = new EventLog("Application", ".", "Enterprise Library Logging");
            var eventLogTraceListener = new FormattedEventLogTraceListener(eventLog, briefFormatter);
            loggingConfiguration.AddLogSource("General", SourceLevels.All, true, eventLogTraceListener);
            loggingConfiguration.SpecialSources.LoggingErrorsAndWarnings.Listeners.Add(eventLogTraceListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(eventLogTraceListener);
        }

        private void UpdateConfigForEventlogSourceFormatter(LoggingConfiguration loggingConfiguration)
        {
            var eventLogTraceListener = new FormattedEventLogTraceListener("Enterprise Library Logging", briefFormatter);
            loggingConfiguration.AddLogSource("General", SourceLevels.All, true, eventLogTraceListener);
            loggingConfiguration.SpecialSources.LoggingErrorsAndWarnings.Listeners.Add(eventLogTraceListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(eventLogTraceListener);
        }

        private void UpdateConfigForEventlogSourceLogFormatter(LoggingConfiguration loggingConfiguration)
        {
            var eventLogTraceListener = new FormattedEventLogTraceListener("Enterprise Library Logging", "Application", briefFormatter);
            loggingConfiguration.AddLogSource("General", SourceLevels.All, true, eventLogTraceListener);
            loggingConfiguration.SpecialSources.LoggingErrorsAndWarnings.Listeners.Add(eventLogTraceListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(eventLogTraceListener);
        }

        private void UpdateConfigForEventlogSourceLogMachineFormatter(LoggingConfiguration loggingConfiguration)
        {
            var eventLogTraceListener = new FormattedEventLogTraceListener("Enterprise Library Logging", "Application", ".", briefFormatter);
            loggingConfiguration.AddLogSource("General", SourceLevels.All, true, eventLogTraceListener);
            loggingConfiguration.SpecialSources.LoggingErrorsAndWarnings.Listeners.Add(eventLogTraceListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(eventLogTraceListener);
        }

        [TestMethod]
        public void EntryIsWrittenToEventlogWhenGeneralCategory()
        {
            string guid = Guid.NewGuid().ToString();
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEventlog(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging: " + guid);
            this.writer.Dispose();

            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Test Logging: " + guid));
        }

        [TestMethod]
        public void EntryIsWrittenToEventlogWhenGeneralCategoryWithFormatter()
        {
            string guid = Guid.NewGuid().ToString();
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEventlogWithFormatter(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging: " + guid);
            this.writer.Dispose();

            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Test Logging: " + guid));
        }

        [TestMethod]
        public void EntryIsWrittenToEventlogWhenGeneralCategorySourceFormatter()
        {
            string guid = Guid.NewGuid().ToString();
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEventlogSourceFormatter(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging: " + guid);
            this.writer.Dispose();

            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Test Logging: " + guid));
        }

        [TestMethod]
        public void EntryIsWrittenToEventlogWhenGeneralCategorySourceLogFormatter()
        {
            string guid = Guid.NewGuid().ToString();
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEventlogSourceLogFormatter(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging: " + guid);
            this.writer.Dispose();

            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Test Logging: " + guid));
        }

        [TestMethod]
        public void EntryIsWrittenToEventlogWhenGeneralCategorySourceLogMachineFormatter()
        {
            string guid = Guid.NewGuid().ToString();
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEventlogSourceLogMachineFormatter(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging: " + guid);
            this.writer.Dispose();

            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Test Logging: " + guid));
        }

        [TestMethod]
        public void EntryIsNotWrittenToEventlogWhenLogEnabledFilterIsFalse()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEventlog(loggingConfiguration);
            var logEnabledFilter = new LogEnabledFilter("LogEnabled Filter", false);
            loggingConfiguration.Filters.Add(logEnabledFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging Not Present");
            this.writer.Dispose();

            Assert.IsFalse(this.CheckForEntryInEventlog("Message: Test Logging Not Present"));
        }

        [TestMethod]
        public void OnlyEntriesInPriorityRangeAreWrittenWhenEventlogWithPriorityFilter()
        {
            string guid = Guid.NewGuid().ToString();
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEventlog(loggingConfiguration);
            var priorityFilter = new PriorityFilter("Priority Filter", 2, 99);
            loggingConfiguration.Filters.Add(priorityFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging PriorityFilter 1: " + guid, "General", 1);
            this.writer.Write("Test Logging PriorityFilter 2: " + guid, "General", 2);
            this.writer.Write("Test Logging PriorityFilter 101: " + guid, "General", 101);
            this.writer.Dispose();

            Assert.IsFalse(this.CheckForEntryInEventlog("Message: Test Logging PriorityFilter 1: " + guid));
            Assert.IsFalse(this.CheckForEntryInEventlog("Message: Test Logging PriorityFilter 101: " + guid));
            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Test Logging PriorityFilter 2: " + guid));
        }

        [TestMethod]
        public void OnlyEntriesWithAppropriateCategoryAreWrittenToEventLogWhenCategoryFilter()
        {
            string guid = Guid.NewGuid().ToString();
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEventlog(loggingConfiguration);

            // Category Filters
            ICollection<string> categories = new List<string>();
            categories.Add("BlockedByFilter");
            var categoryFilter = new CategoryFilter("Category Filter", categories, CategoryFilterMode.DenyAllExceptAllowed);
            loggingConfiguration.Filters.Add(categoryFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging CategoryFilter 1: " + guid, "General", 1);
            this.writer.Write("Test Logging CategoryFilter 2: " + guid, "BlockedByFilter", 2);
            this.writer.Dispose();

            Assert.IsFalse(this.CheckForEntryInEventlog("Message: Test Logging CategoryFilter 1: " + guid));
            Assert.IsTrue(this.CheckForEntryInEventlog("Message: Test Logging CategoryFilter 2: " + guid));
        }
    }
}