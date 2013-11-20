// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Microsoft.Practices.EnterpriseLibrary.Logging
{
    /// <summary>
    /// Error handling delegate.
    /// </summary>
    /// <param name="exception">The exception that was thrown while tracing.</param>
    /// <param name="data">The data.</param>
    /// <param name="source">The source.</param>
    public delegate void ReportTracingError(Exception exception, object data, string source);
}
