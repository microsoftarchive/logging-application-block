// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Helpers;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Tests
{
    internal class MockDebugUtilsThrowsNonSecurityException : IDebugUtils
    {
        public string GetStackTraceWithSourceInfo(StackTrace stackTrace)
        {
            throw new NotImplementedException();
        }
    }
}

