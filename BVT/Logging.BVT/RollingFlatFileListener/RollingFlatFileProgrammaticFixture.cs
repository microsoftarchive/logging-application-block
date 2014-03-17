using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.RollingFlatFileListener
{
    [TestClass]
    public class RollingFlatFileProgrammaticConfigurationFixture : LoggingFixtureBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            this.strPath = @".\LogConfig" + Guid.NewGuid().ToString();
            LogFileReader.CreateDirectory(this.strPath);
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        private void UpdateConfigForRollingFlatFileRollExistsIncrement(LoggingConfiguration loggingConfiguration)
        {
            var rollingFlatFileTraceListener = new RollingFlatFileTraceListener(Path.Combine(strPath, "RollingFlatFile.log"), "----------------------------------------", "----------------------------------------", extendedFormatter, 1, "yyyy", RollFileExistsBehavior.Increment, RollInterval.None, 3);
            loggingConfiguration.AddLogSource("RollFFIncrement", SourceLevels.All, true, rollingFlatFileTraceListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(rollingFlatFileTraceListener);
        }

        private void UpdateConfigForRollingFlatFileRollExistsOverwrite(LoggingConfiguration loggingConfiguration)
        {
            var rollingFlatFileTraceListener = new RollingFlatFileTraceListener(Path.Combine(strPath, "RollingFlatFile.log"), "----------------------------------------", "----------------------------------------", extendedFormatter, 0, String.Empty, RollFileExistsBehavior.Overwrite, RollInterval.None, 3);
            loggingConfiguration.AddLogSource("RollFFOverwrite", SourceLevels.All, true, rollingFlatFileTraceListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(rollingFlatFileTraceListener);
        }

        [TestMethod]
        public void ArchiveFilesAreWrittenWhenAddingUnprocessedSourceForRollingFlatFile()
        {
            var config = new LoggingConfiguration();
            config.IsTracingEnabled = true;
            config.DefaultSource = "General";
            config.LogWarningsWhenNoCategoriesMatch = true;

            var rollingFlatFileTraceListener = new RollingFlatFileTraceListener(Path.Combine(strPath, @"TraceTest.log"),
                "----------------------------------------",
                "----------------------------------------",
                new TextFormatter("Timestamp: {timestamp}{newline}&#xA;Message: {message}{newline}&#xA;Category: {category}{newline}&#xA;Priority: {priority}{newline}&#xA;EventId: {eventid}{newline}&#xA;Severity: {severity}{newline}&#xA;Title:{title}{newline}&#xA;Machine: {localMachine}{newline}&#xA;App Domain: {localAppDomain}{newline}&#xA;ProcessId: {localProcessId}{newline}&#xA;Process Name: {localProcessName}{newline}&#xA;Thread Name: {threadName}{newline}&#xA;Win32 ThreadId:{win32ThreadId}{newline}&#xA;Extended Properties: {dictionary({key} - {value}{newline})}"),
                1, "yyyy", RollFileExistsBehavior.Increment, RollInterval.None, 3);

            config.SpecialSources.Unprocessed.Listeners.Add(rollingFlatFileTraceListener);
            this.writer = new LogWriter(config);

            this.writer.Write("Test Logging Rolling 1");
            this.writer.Write("Test Logging Rolling 2");
            this.writer.Write("Test Logging Rolling 3");
            this.writer.Write("Test Logging Rolling 4");
            this.writer.Write("Test Logging Rolling 5");
            this.writer.Write("Test Logging Rolling 6");
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "TraceTest.log")));
            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "TraceTest" + "." + DateTime.Now.Year + ".1" + ".log")));
            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "TraceTest" + "." + DateTime.Now.Year + ".2" + ".log")));
        }

        [TestMethod]
        public void RollingFlatFileGeneralCategoryIncrement()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForRollingFlatFileRollExistsIncrement(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            for (int i = 0; i < 10; i++)
            {
                this.writer.Write("Test Logging Rolling " + i.ToString());
            }
            this.writer.Dispose();

            Assert.AreEqual(4, Directory.GetFiles(this.strPath).Length);
            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "RollingFlatFile.log")));
            Assert.IsFalse(File.Exists(Path.Combine(this.strPath, "RollingFlatFile" + "." + DateTime.Now.Year + ".1" + ".log")));
            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "RollingFlatFile" + "." + DateTime.Now.Year + ".2" + ".log")));
            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "RollingFlatFile" + "." + DateTime.Now.Year + ".3" + ".log")));
            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "RollingFlatFile" + "." + DateTime.Now.Year + ".4" + ".log")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(strPath, "RollingFlatFile.log"));

            Assert.IsTrue(strFileData.Contains("Message: Test Logging Rolling 8"));
            Assert.IsTrue(strFileData.Contains("Message: Test Logging Rolling 9"));
            Assert.IsTrue(strFileData.Contains("Category: General"));
        }

        [TestMethod]
        public void EntryIsWrittenAndOneFileExistsWhenRollingFlatFileGeneralCategoryOverwrite()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForRollingFlatFileRollExistsOverwrite(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            for (int i = 0; i < 10; i++)
            {
                this.writer.Write("Test Logging Rolling " + i.ToString());
            }
            this.writer.Dispose();

            Assert.AreEqual(1, Directory.GetFiles(this.strPath).Length);
            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "RollingFlatFile.log")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "RollingFlatFile.log"));
            Assert.IsTrue(strFileData.Contains("Message: Test Logging Rolling 8"));
            Assert.IsTrue(strFileData.Contains("Message: Test Logging Rolling 9"));
            Assert.IsTrue(strFileData.Contains("Category: General"));
        }

        [TestMethod]
        public void EntryIsNotWrittenWhenRollingFlatFileLogEnabledFilterIsFalse()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForRollingFlatFileRollExistsIncrement(loggingConfiguration);
            var logEnabledFilter = new LogEnabledFilter("LogEnabled Filter", false);
            loggingConfiguration.Filters.Add(logEnabledFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging Not Present");
            this.writer.Dispose();

            Assert.IsFalse(File.Exists(Path.Combine(this.strPath, "RollingFlatFile.log")));
        }

        [TestMethod]
        public void OnlyEntriesInPriorityRangeAreWrittenWhenRollingFlatFilePriorityFilter()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForRollingFlatFileRollExistsIncrement(loggingConfiguration);

            var priorityFilter = new PriorityFilter("Priority Filter", 2, 99);
            loggingConfiguration.Filters.Add(priorityFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging1", "General", 1);
            this.writer.Write("Test Logging2", "General", 2);
            this.writer.Write("Test Logging101", "General", 101);
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "RollingFlatFile.log")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "RollingFlatFile.log"));
            
            Assert.IsFalse(strFileData.Contains("Message: Test Logging1"));
            Assert.IsFalse(strFileData.Contains("Message: Test Logging101"));
            Assert.IsTrue(strFileData.Contains("Message: Test Logging2"));
        }

        [TestMethod]
        public void OnlyApplicableEntriesAreWrittenWhenRollingFlatFileCategoryFilter()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForRollingFlatFileRollExistsIncrement(loggingConfiguration);

            // Category Filters
            ICollection<string> categories = new List<string>();
            categories.Add("BlockedByFilter");
            var categoryFilter = new CategoryFilter("Category Filter", categories, CategoryFilterMode.DenyAllExceptAllowed);
            loggingConfiguration.Filters.Add(categoryFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging1", "General", 1);
            this.writer.Write("Test Logging2", "BlockedByFilter", 2);
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "RollingFlatFile.log")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "RollingFlatFile.log"));

            Assert.IsFalse(strFileData.Contains("Message: Test Logging1"));
            Assert.IsTrue(strFileData.Contains("Message: Test Logging2"));
        }
    }
}