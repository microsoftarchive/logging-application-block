// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Filters
{
    /// <summary>
    /// Contract for handling errors during evaluation of an <see cref="ILogFilter"/>.
    /// </summary>
    public interface ILogFilterErrorHandler
    {
        /// <summary>
        /// Perform any action to handle an error during checking.
        /// </summary>
        /// <param name="ex">The exception raised during filter evaluation.</param>
        /// <param name="logEntry">The log entry being evaluated.</param>
        /// <param name="filter">The fiter that raised the exception.</param>
        /// <returns>True if processing should continue, ignoring the failed filter, or 
        /// false if the filter evaluation should be considered as failed.</returns>
        bool FilterCheckingFailed(Exception ex, LogEntry logEntry, ILogFilter filter);
    }
}
