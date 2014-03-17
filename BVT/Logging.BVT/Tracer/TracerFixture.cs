using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EntLibTracer = Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Tracer
{
    [TestClass]
    public class TracerFixture : LoggingFixtureBase
    {
        private const string Operation = "operation";
        private const string FilePath = "tracer.log";

        public TracerFixture()
            : base("Tracer.Tracer.config")
        {
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(LoggingSettings.SectionName));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        [TestMethod]
        public void TraceManagerIsResolved()
        {
            var traceManager = new TraceManager(writer);
            Assert.IsNotNull(traceManager);
        }

        [TestMethod]
        public void TraceIsWrittenWhenStarted()
        {            
            var traceManager = new TraceManager(writer);

            Assert.IsNotNull(traceManager);

            var tracer = traceManager.StartTrace(Operation);

            Assert.IsNotNull(tracer);

            FileInfo info = new FileInfo(FilePath);

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void ActivityIdIsResetWhenTracerIsDisposed() 
        {
            Trace.CorrelationManager.ActivityId = Guid.Empty;

            using (new EntLibTracer.Tracer("test"))
            {
                // nest tracer
                using (new EntLibTracer.Tracer("test2"))
                { }
            }
            
            Assert.AreEqual(Guid.Empty, Trace.CorrelationManager.ActivityId);
        }
    }
}