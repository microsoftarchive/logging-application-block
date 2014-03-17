using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Extensibility
{
    [TestClass]
    public class CustomTraceListenerFixture : EntLibFixtureBase
    {
        private LogWriter writer = null;

        public CustomTraceListenerFixture() :
            base("Extensibility.CustomTraceListener.config")
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(e));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);
        }

        [TestCleanup]
        public override void Cleanup()
        {
            Logger.Reset();
            this.writer.Dispose();
            base.Cleanup();
        }

        [TestMethod]
        public void CustomTraceListenerIsInvoked()
        {
            MyCustomTraceListener.Wrote = false;
            MyCustomTraceListener.WroteLine = false;

            var listener =
                this.writer.TraceSources.Values.SelectMany(x => x.Listeners).OfType<MyCustomTraceListener>().Single();

            Assert.IsFalse(MyCustomTraceListener.Wrote);

            LogEntry entry = LogEntryFactory.GetLogEntry();
            entry.Categories.Add("General");
            this.writer.Write(entry);

            Assert.IsTrue(MyCustomTraceListener.Wrote);
        }

        [TestMethod]
        public void CustomFormatterIsInvoked()
        {
            var listener =
                this.writer.TraceSources.Values.SelectMany(x => x.Listeners).OfType<MyCustomTraceListener>().Single();
            
            var customFormatter = listener.Formatter as CustomFormatter;

            Assert.IsFalse(customFormatter.FormattedInvoked);

            this.writer.Write(LogEntryFactory.GetLogEntry());

            Assert.IsTrue(customFormatter.FormattedInvoked);
        }
    }
}