// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.Tests
{
    public class ExceptionThrowingMockTraceListener : TraceListener
    {
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            throw new Exception("exception while tracing");
        }

        public override void Write(string message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteLine(string message)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
