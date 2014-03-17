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

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.XmlListener
{
    [TestClass]
    public class XmlProgrammaticConfigurationFixture : LoggingFixtureBase
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

        public void UpdateConfigForXMLTL(LoggingConfiguration loggingConfiguration)
        {
            var xmlTraceListener = new XmlTraceListener(Path.Combine(strPath, "XmlLogFile.xml"));
            xmlTraceListener.Filter = new EventTypeFilter(SourceLevels.All);
            loggingConfiguration.IsTracingEnabled = false;
            loggingConfiguration.AddLogSource("XML", SourceLevels.All, true, xmlTraceListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(xmlTraceListener);
        }

        [TestMethod]
        public void EntryIsWrittenWhenUsingXmlTraceListener()
        {
            File.Delete("LogFile.xml");
            
            var config = new LoggingConfiguration();
            config.IsTracingEnabled = true;
            config.DefaultSource = "General";
            config.LogWarningsWhenNoCategoriesMatch = true;

            var xmlTraceListener = new XmlTraceListener(@"LogFile.xml");
            config.SpecialSources.Unprocessed.Listeners.Add(xmlTraceListener);

            this.writer = new LogWriter(config);
            this.writer.Write("Test Logging XML");
            this.writer.Dispose();

            Assert.IsTrue(File.Exists("LogFile.xml"));
        }

        [TestMethod]
        public void EntryIsWrittenWhenXMLGeneralCategory()
        {
            File.Delete(Path.Combine(this.strPath, "XmlLogFile.xml"));

            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForXMLTL(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging XML", "General");

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "XmlLogFile.xml")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "XmlLogFile.xml"));
            
            Assert.IsTrue(strFileData.Contains("<Message>Test Logging XML</Message>"));
            Assert.IsTrue(strFileData.Contains("<Categories><String>General</String></Categories>"));
        }

        [TestMethod]
        public void EntryIsNotWrittenWhenXMLLogEnabledFilterIsFalse()
        {
            File.Delete(Path.Combine(this.strPath, "XmlLogFile.xml"));

            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForXMLTL(loggingConfiguration);
            var logEnabledFilter = new LogEnabledFilter("LogEnabled Filter", false);
            loggingConfiguration.Filters.Add(logEnabledFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging XML Not Present");
            this.writer.Dispose();

            Assert.IsFalse(File.Exists(Path.Combine(this.strPath, "XmlLogFile.xml")));
        }

        [TestMethod]
        public void OnlyEntriesInPriorityRangeAreWrittenWhenXmlListenerPriorityFilter()
        {
            File.Delete(Path.Combine(this.strPath, "XmlLogFile.xml"));

            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForXMLTL(loggingConfiguration);
            var priorityFilter = new PriorityFilter("Priority Filter", 2, 99);
            loggingConfiguration.Filters.Add(priorityFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging XML 1", "General", 1);
            this.writer.Write("Test Logging XML 2", "General", 2);
            this.writer.Write("Test Logging XML 101", "General", 101);
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "XmlLogFile.xml")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "XmlLogFile.xml"));

            Assert.IsFalse(strFileData.Contains("<Message>Test Logging XML 1</Message>"));
            Assert.IsFalse(strFileData.Contains("<Message>Test Logging XML 101</Message>"));
            Assert.IsTrue(strFileData.Contains("<Message>Test Logging XML 2</Message>"));
        }

        [TestMethod]
        public void OnlyApplicableEntriesAreWrittenWhenXmlListenerCategoryFilter()
        {
            File.Delete(Path.Combine(this.strPath, "XmlLogFile.xml"));

            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForXMLTL(loggingConfiguration);

            // Category Filters
            ICollection<string> categories = new List<string>();
            categories.Add("BlockedByFilter");
            var categoryFilter = new CategoryFilter("Category Filter", categories, CategoryFilterMode.DenyAllExceptAllowed);
            loggingConfiguration.Filters.Add(categoryFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging XML 1", "General", 1);
            this.writer.Write("Test Logging XML 2", "BlockedByFilter", 2);
            this.writer.Dispose();

            Assert.IsTrue(File.Exists(Path.Combine(this.strPath, "XmlLogFile.xml")));

            string strFileData = LogFileReader.ReadFileWithoutLock(Path.Combine(this.strPath, "XmlLogFile.xml"));

            Assert.IsFalse(strFileData.Contains("<Message>Test Logging XML 1</Message>"));
            Assert.IsTrue(strFileData.Contains("<Message>Test Logging XML 2</Message>"));
        }
    }
}