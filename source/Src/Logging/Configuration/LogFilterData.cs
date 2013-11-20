// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration
{
    /// <summary>
    /// Base class for <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Filters.ILogFilter"/> configuration objects.
    /// </summary>
    /// <remarks>
    /// This class should be made abstract, but in order to use it in a NameTypeConfigurationElementCollection
    /// it must be public and have a no-args constructor.
    /// </remarks>
    public class LogFilterData : NameTypeConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LogFilterData"/>.
        /// </summary>
        public LogFilterData()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LogFilterData"/> with name and type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public LogFilterData(string name, Type type)
            : base(name, type)
        {
        }

        /// <summary>
        /// Builds the <see cref="ILogFilter" /> object represented by this configuration object.
        /// </summary>
        /// <returns>
        /// A filter.
        /// </returns>
        public virtual ILogFilter BuildFilter()
        {
            throw new NotImplementedException(Resources.ExceptionMethodMustBeImplementedBySubclasses);
        }
    }
}
