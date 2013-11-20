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
    /// Fluent interface that allows tracelisteners to be configured.
    /// </summary>
    public interface ILoggingConfigurationSendTo : IFluentInterface
    {
        /// <summary>
        /// Creates a reference to an existing Trace Listener with a specific name.
        /// </summary>
        /// <param name="listenerName">The name of the Trace Listener a reference should be made for.</param>
        ILoggingConfigurationCategoryContd SharedListenerNamed(string listenerName);

    }

}
