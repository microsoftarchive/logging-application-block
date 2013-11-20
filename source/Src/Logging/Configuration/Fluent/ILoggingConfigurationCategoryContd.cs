// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using System.Diagnostics;
using System.Messaging;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using System.Collections.Specialized;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent
{
    /// <summary>
    /// Fluent interface that allows tracelisteners to be configured for a Category Source.
    /// </summary>
    public interface ILoggingConfigurationCategoryContd : ILoggingConfigurationContd, IFluentInterface
    {
        /// <summary>
        /// Entry point for attaching Trace Listeners to a Category Source.
        /// </summary>
        ILoggingConfigurationSendTo SendTo { get; }
    }
}
