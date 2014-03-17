using System;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Instrumentation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT
{
    [TestClass]
    public class DistributorEventLoggerFixture
    {
        private const string EventSourceName = "Sample";

        [TestInitialize]
        public void StartUp()
        {
            if (EventLog.Exists(EventSourceName))
            {
                EventLog.DeleteEventSource(EventSourceName);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenCreatingAnEmptyDistributorEventLogger()
        {
            DistributorEventLogger eventLogger = new DistributorEventLogger(string.Empty);
        }

        [TestMethod]
        public void EventSourceExistsWhenDistributedServiceStarted()
        {
            DistributorEventLogger eventLogger = new DistributorEventLogger(EventSourceName);

            eventLogger.LogServiceStarted();

            Assert.IsTrue(EventLog.SourceExists(eventLogger.EventSource));
        }

        [TestMethod]
        public void EventSourceExistsWhenDistributedServiceResumed()
        {
            DistributorEventLogger eventLogger = new DistributorEventLogger(EventSourceName);

            eventLogger.LogServiceResumed();

            Assert.IsTrue(EventLog.SourceExists(eventLogger.EventSource));
        }

        [TestMethod]
        public void EventSourceExistsWhenDistributedServiceStopped()
        {
            DistributorEventLogger eventLogger = new DistributorEventLogger(EventSourceName);

            eventLogger.LogServiceStopped();

            Assert.IsTrue(EventLog.SourceExists(eventLogger.EventSource));
        }

        [TestMethod]
        public void EventSourceExistsWhenDistributedServicePaused()
        {
            DistributorEventLogger eventLogger = new DistributorEventLogger(EventSourceName);

            eventLogger.LogServicePaused();

            Assert.IsTrue(EventLog.SourceExists(eventLogger.EventSource));
        }

        [TestMethod]
        public void EventSourceExistsWhenDistributedServiceFailure()
        {
            DistributorEventLogger eventLogger = new DistributorEventLogger(EventSourceName);

            eventLogger.LogServiceFailure("Failure", new Exception(), TraceEventType.Critical);

            Assert.IsTrue(EventLog.SourceExists(eventLogger.EventSource));
        }
    }
}