// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Formatters.Tests
{
#pragma warning disable 618
    public class CustomToken : TokenFunction
    {
        public CustomToken() : base("[[AcmeDBLookup{", "}]]")
        {
        }

        public override string FormatToken(string tokenTemplate, LogEntry log)
        {
            return "1234";
        }
    }
#pragma warning restore 618
}

