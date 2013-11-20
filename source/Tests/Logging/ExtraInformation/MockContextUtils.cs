// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Runtime.InteropServices;
using Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Helpers;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Tests
{
    public class MockContextUtils : IContextUtils
    {
        public string GetActivityId()
        {
            throw new COMException();
        }

        public string GetApplicationId()
        {
            throw new COMException();
        }

        public string GetTransactionId()
        {
            throw new COMException();
        }

        public string GetDirectCallerAccountName()
        {
            throw new COMException();
        }

        public string GetOriginalCallerAccountName()
        {
            throw new COMException();
        }
    }
}

