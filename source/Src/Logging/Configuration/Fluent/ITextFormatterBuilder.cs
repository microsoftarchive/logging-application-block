// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent
{
    /// <summary>
    /// Fluent interface used to configure a <see cref="TextFormatter"/> instance.
    /// </summary>
    public interface ITextFormatterBuilder : IFormatterBuilder, IFluentInterface
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        IFormatterBuilder UsingTemplate(string template);
    }
}
