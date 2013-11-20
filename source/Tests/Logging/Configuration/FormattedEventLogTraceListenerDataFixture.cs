// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Tests.TestSupport;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Tests
{
    [TestClass]
    public class GivenFormattedEventLogTraceListenerDataWithFilterData
    {
        private TraceListenerData listenerData;

        [TestInitialize]
        public void Setup()
        {
            listenerData =
                new FormattedEventLogTraceListenerData("listener", "source", "log", "machine", "formatter")
                    {
                        TraceOutputOptions = TraceOptions.DateTime | TraceOptions.Callstack,
                        Filter = SourceLevels.Warning
                    };
        }

        [TestMethod]
        public void WhenCreatingListener_ThenCreatesEventLogListener()
        {
            var settings = new LoggingSettings { Formatters = { new TextFormatterData { Name = "formatter", Template = "template" } } };

            var listener = (FormattedEventLogTraceListener)listenerData.BuildTraceListener(settings);

            Assert.IsNotNull(listener);
            Assert.AreEqual("listener", listener.Name);
            Assert.AreEqual(TraceOptions.DateTime | TraceOptions.Callstack, listener.TraceOutputOptions);
            Assert.IsNotNull(listener.Filter);
            Assert.AreEqual(SourceLevels.Warning, ((EventTypeFilter)listener.Filter).EventType);
            Assert.IsInstanceOfType(listener.Formatter, typeof(TextFormatter));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNull()
        {
            string name = null;

            var listener = new FormattedEventLogTraceListener(name);
        }
    }
}
