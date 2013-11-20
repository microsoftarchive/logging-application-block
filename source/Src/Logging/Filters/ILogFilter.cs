// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Filters
{
    /// <summary>
    /// Represents the interface for client-side message filters.
    /// </summary>
    public interface ILogFilter
    {
        /// <summary>
        /// Test to see if a message meets the criteria to be processed. 
        /// </summary>
        /// <param name="log">Log entry to test.</param>
        /// <returns><b>true</b> if the message passes through the filter and should be distributed, <b>false</b> otherwise.</returns>
        bool Filter(LogEntry log);

        /// <summary>
        /// Gets the name of the log filter
        /// </summary>
        string Name { get; }
    }
}
