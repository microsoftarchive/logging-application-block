// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Specialized;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.TestSupport.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Formatters.Tests
{
    [ConfigurationElementType(typeof(CustomFormatterData))]
    public class MockCustomLogFormatter
        : MockCustomProviderBase, ILogFormatter
    {
        public MockCustomLogFormatter(NameValueCollection attributes)
            : base(attributes)
        {
        }

        public string Format(LogEntry log)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
