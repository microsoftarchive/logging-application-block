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
    /// Allows access to the configuration classes used to configure <see cref="TraceListener"/> instances.
    /// </summary>
    public interface ILoggingConfigurationSendToExtension : ILoggingConfigurationExtension, IFluentInterface
    {
        /// <summary>
        /// Returns an interface that can be used to configure a logging category.
        /// </summary>
        ILoggingConfigurationCategoryContd LoggingCategoryContd { get; }

        /// <summary>
        /// Returns the logging category configuration currently being build up.
        /// </summary>
        TraceSourceData CurrentTraceSource { get; }

    }
}
