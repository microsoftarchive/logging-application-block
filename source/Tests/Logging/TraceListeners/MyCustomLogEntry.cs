// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.Tests
{
    public class MyCustomLogEntry : LogEntry
    {
        private string myName;

        public MyCustomLogEntry()
        {
            myName = "MyCustomLogEntry";
        }

        public string MyName
        {
            get { return myName; }
            set { myName = value; }
        }
    }
}

