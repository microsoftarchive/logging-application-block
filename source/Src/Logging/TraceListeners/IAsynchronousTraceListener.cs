// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
    /// <summary>
    /// Implements asynchronous tracing.
    /// </summary>
    public interface IAsynchronousTraceListener
    {
        /// <summary>
        /// Flushes the output buffer asynchronously.
        /// </summary>
        /// <param name="reportError">The delegate to use to report errors while tracing asynchronously.</param>
        void Flush(ReportTracingError reportError);

        /// <summary>
        /// Writes trace information, a data object and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">The trace data to emit.</param>
        /// <param name="reportError">The delegate to use to report errors while tracing asynchronously.</param>
        void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data, ReportTracingError reportError);

        /// <summary>
        /// Writes trace information, a message, a related activity identity and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        /// <param name="relatedActivityId">A <see cref="T:System.Guid" />  object identifying a related activity.</param>
        /// <param name="reportError">The delegate to use to report errors while tracing asynchronously.</param>
        void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId, ReportTracingError reportError);
    }
}
