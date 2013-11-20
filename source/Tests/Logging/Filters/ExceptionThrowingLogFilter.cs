// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Filters.Tests
{
    class ExceptionThrowingLogFilter : ILogFilter
    {
        private string name;

        internal ExceptionThrowingLogFilter(string name)
        {
            this.name = name;
        }

        public bool Filter(LogEntry log)
        {
            throw new Exception("exception during evaluation.");
        }

        public string Name
        {
            get { return name; }
        }
    }
}
