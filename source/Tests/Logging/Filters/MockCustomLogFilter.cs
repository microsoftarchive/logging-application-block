// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Specialized;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.TestSupport.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Filters.Tests
{
    [ConfigurationElementType(typeof(CustomLogFilterData))]
    public class MockCustomLogFilter : MockCustomProviderBase, ILogFilter
    {
        public MockCustomLogFilter(NameValueCollection attributes)
            : base(attributes)
        {
        }

        public bool Filter(LogEntry log)
        {
            return true;
        }

        public string Name
        {
            get { return string.Empty; }
        }
    }
}
