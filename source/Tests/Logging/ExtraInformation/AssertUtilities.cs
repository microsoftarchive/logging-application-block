// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Tests
{
    static class AssertUtilities
    {
        internal static void AssertStringDoesNotContain(string o,
                                                        string s,
                                                        string message)
        {
            Assert.IsNotNull(o);
            Assert.IsNotNull(s);
            Assert.IsNotNull(message);
            Assert.IsFalse(o.StartsWith(s), string.Format("\nIn {2}, the string:\n\t{0}\ncontains\n\t{1}\nwhen it should not.\n", o, s, message));
        }
    }
}
