// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Security;
using Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Helpers;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Tests
{
    /// <summary>
    /// Summary description for MockDebugUtils.
    /// </summary>
    public class MockDebugUtilsThrowsSecurityException : IDebugUtils
    {
        public string GetStackTraceWithSourceInfo(StackTrace stackTrace)
        {
            throw new SecurityException();
        }
    }
}

