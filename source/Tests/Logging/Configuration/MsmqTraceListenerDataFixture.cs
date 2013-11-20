// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Messaging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Tests
{
    [TestClass]
    public class GivenMsmqTraceListenerDataWithFilterData
    {
        private TraceListenerData listenerData;

        [TestInitialize]
        public void Setup()
        {
            listenerData =
                new MsmqTraceListenerData(
                    "listener",
                    "queue path",
                    "formatter",
                    MessagePriority.High,
                    true,
                    TimeSpan.MinValue,
                    TimeSpan.MaxValue,
                    false,
                    true,
                    false,
                    MessageQueueTransactionType.Automatic)
                    {
                        TraceOutputOptions = TraceOptions.DateTime | TraceOptions.Callstack,
                        Filter = SourceLevels.Warning
                    };
        }

        [TestMethod]
        public void WhenCreatingInstanceUsingDefaultContructor_ThenListenerDataTypeIsSet()
        {
            var listener = new MsmqTraceListenerData();
            Assert.AreEqual(typeof(MsmqTraceListenerData), listener.ListenerDataType);
        }

        [TestMethod]
        public void WhenCreatingListener_ThenCreatesMsmqListener()
        {
            var settings = new LoggingSettings { Formatters = { new BinaryLogFormatterData { Name = "formatter" } } };

            var listener = (MsmqTraceListener)listenerData.BuildTraceListener(settings);

            Assert.IsNotNull(listener);
            Assert.AreEqual("listener", listener.Name);
            Assert.AreEqual(TraceOptions.DateTime | TraceOptions.Callstack, listener.TraceOutputOptions);
            Assert.IsNotNull(listener.Filter);
            Assert.AreEqual(SourceLevels.Warning, ((EventTypeFilter)listener.Filter).EventType);
            Assert.IsInstanceOfType(listener.Formatter, typeof(BinaryLogFormatter));
            Assert.AreEqual("queue path", listener.QueuePath);
        }
    }
}
