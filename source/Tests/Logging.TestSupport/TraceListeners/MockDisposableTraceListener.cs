// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TestSupport.TraceListeners
{
    public class MockDisposableTraceListener : TraceListener, IDisposable
    {
        public int DisposedCalls;

        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
        }

        public new void Dispose()
        {
            this.DisposedCalls++;
        }
    }
}
