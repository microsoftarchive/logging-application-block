// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Formatters
{
    /// <summary>
    /// Specifies formatting options.
    /// </summary>
    public enum JsonFormatting
    {
        /// <summary>
        /// Specifies that no special formatting should be applied. This is the default.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that child objects should be indented.
        /// </summary>
        Indented
    }
}
