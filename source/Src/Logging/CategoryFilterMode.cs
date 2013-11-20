// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.EnterpriseLibrary.Logging
{
    /// <summary>
    /// Represents the possible modes for client-side category filtering.
    /// </summary>
    public enum CategoryFilterMode
    {
        /// <summary>
        /// Allow all categories except those explicitly denied
        /// </summary>
        AllowAllExceptDenied,

        /// <summary>
        /// Deny all categories except those explicitly allowed
        /// </summary>
        DenyAllExceptAllowed
    }
}
