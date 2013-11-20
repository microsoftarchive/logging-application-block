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
    /// Fluent interface that allows settings to be configured for a Category Source.
    /// </summary>
    public interface ILoggingConfigurationCategoryStart : ILoggingConfigurationCategoryContd, IFluentInterface
    {
        /// <summary>
        /// Returns a fluent interface for further configuring a logging category.
        /// </summary>
        ILoggingConfigurationCategoryOptions WithOptions { get; }
    }
}
