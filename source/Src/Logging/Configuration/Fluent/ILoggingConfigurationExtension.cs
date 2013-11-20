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
    /// Allows access to the internal configuration classes used to configure the logging application block.
    /// </summary>
    public interface ILoggingConfigurationExtension : IFluentInterface
    {
        /// <summary>
        /// Returns a fluent interface that can be used to configure global logging application block settings.
        /// </summary>
        ILoggingConfigurationOptions LoggingOptions { get; }

        /// <summary>
        /// Returns the <see cref="LoggingSettings"/> that are being build up.
        /// </summary>
        LoggingSettings LoggingSettings { get; }
    }
}
