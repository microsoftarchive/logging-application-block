// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TestSupport.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Tests
{
    [TestClass]
    public class TracerManagerFixture
    {
        [TestMethod]
        public void GetTracerFromTraceManagerWithNoInstrumentation()
        {
            MockTraceListener.Reset();

            LogSource source = new LogSource("tracesource", new[] { new MockTraceListener() }, SourceLevels.All);

            List<LogSource> traceSources = new List<LogSource>(new LogSource[] { source });
            LogWriter lg = new LogWriter(new List<ILogFilter>(), new List<LogSource>(), source, null, new LogSource("errors"), "default", true, false);

            TraceManager tm = new TraceManager(lg);

            Assert.IsNotNull(tm);

            using (tm.StartTrace("testoperation"))
            {
                Assert.AreEqual(1, MockTraceListener.Entries.Count);
            }

            Assert.AreEqual(2, MockTraceListener.Entries.Count);
        }
    }
}
