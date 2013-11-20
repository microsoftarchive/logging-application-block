// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
    /// <summary>
    /// Base class for custom trace listeners that support formatters.
    /// </summary>
    public abstract class CustomTraceListener : TraceListener
    {
        private ILogFormatter formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTraceListener"/> class.
        /// </summary>
        protected CustomTraceListener()
        {
        }

        /// <summary>
        /// Gets or sets the log entry formatter.
        /// </summary>
        public ILogFormatter Formatter
        {
            get { return this.formatter; }
            set { this.formatter = value; }
        }
    }
}
