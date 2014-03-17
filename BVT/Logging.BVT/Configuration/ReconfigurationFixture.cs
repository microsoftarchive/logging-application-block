using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Configuration
{
    [TestClass]
    public class ReconfigurationFixture : LoggingFixtureBase
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

        [TestMethod]
        public void EntryIsLoggedWhenLoggingLevelIsChanged()
        {
            string fileName = "trace.log";

            File.Delete(fileName);

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration72"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            entry.Severity = System.Diagnostics.TraceEventType.Verbose;
            entry.Categories.Clear();
            entry.Categories.Add("General");

            this.writer.Write(entry);

            FileInfo info = new FileInfo(fileName);

            Assert.IsFalse(info.Exists);

            this.SetSourceLevel("General", SourceLevels.Critical);

            entry.Severity = TraceEventType.Critical;

            this.writer.Write(entry);

            info = new FileInfo(fileName);

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void WhenCategoryFilterIsChanged()
        {
            string fileName = "trace.log";

            File.Delete(fileName);

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration72"));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            var entry = LogEntryFactory.GetLogEntry();
            entry.Severity = System.Diagnostics.TraceEventType.Critical;
            entry.Categories.Clear();
            entry.Categories.Add("General");

            this.writer.Write(entry);

            FileInfo info = new FileInfo(fileName);

            Assert.IsTrue(info.Exists);
            long originalLength = info.Length;

            this.ReplaceCategoryFilter("General");

            this.writer.Write(entry);

            info = new FileInfo(fileName);

            Assert.IsTrue(info.Length == originalLength);
        }

        private void SetSourceLevel(string category, SourceLevels level)
        {
            this.writer.Configure(cfg =>
                {
                    cfg.LogSources[category].Level = level;
                });
        }

        private void ReplaceCategoryFilter(string category)
        {
            this.writer.Configure(cfg =>
            {
                cfg.Filters.Clear();
                // Category Filters
                ICollection<string> categories = new List<string>() { category };
                categories.Add("BlockedByFilter");
                // Log Filters
                var categoryFilter = new CategoryFilter(
                "Category Filter", categories, CategoryFilterMode.AllowAllExceptDenied);
                cfg.Filters.Add(categoryFilter);
            });
        }
    }
}