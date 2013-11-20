// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Filters.Tests
{
    internal class MockLogFilterErrorHandler : ILogFilterErrorHandler
    {
        internal ICollection<ILogFilter> failingFilters = new List<ILogFilter>();
        private bool returnValue = false;

        internal MockLogFilterErrorHandler(bool returnValue)
        {
            this.returnValue = returnValue;
        }

        public bool FilterCheckingFailed(System.Exception ex, LogEntry logEntry, ILogFilter filter)
        {
            failingFilters.Add(filter);
            return returnValue;
        }
    }
}
