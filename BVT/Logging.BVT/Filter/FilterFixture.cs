using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Filter
{
    [TestClass]
    public class FilterFixture : LoggingFixtureBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration101"));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        [TestMethod]
        public void ShouldNotLogWhenDeniedCategory()
        {
            LogEntry entry = LogEntryFactory.GetLogEntry();
            entry.Categories.Clear();
            entry.Categories.Add("Denied");

            Assert.IsFalse(writer.ShouldLog(entry));
        }

        [TestMethod]
        public void ShouldOnlyLogWhenAllowedCategoryInCategories()
        {
            LogEntry entry = LogEntryFactory.GetLogEntry();

            entry.Categories.Clear();
            entry.Categories.Add("RFFBehavIncrTimeStampEmpty");

            Assert.IsFalse(Logger.ShouldLog(entry));

            entry.Categories.Add("General");
            
            Assert.IsTrue(Logger.ShouldLog(entry));
        }

        [TestMethod]
        public void CorrectFilterCategoryExists()
        {
            var filt = Logger.GetFilter<LogFilter>("Category");

            CategoryFilter filter = (CategoryFilter)filt;
            Assert.IsNotNull(filter);

            Assert.AreEqual("TestValidXML", filter.CategoryFilters.ToArray()[0]);
        }
    }
}
