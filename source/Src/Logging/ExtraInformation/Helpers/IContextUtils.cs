// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Security;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Helpers
{
    /// <summary>
    /// Contract for accessing context information.
    /// </summary>
    [SecurityCritical]
    public interface IContextUtils
    {
        /// <summary>
        /// Returns the ActivityId.
        /// </summary>
        /// <returns>The ActivityId</returns>
        string GetActivityId();

        /// <summary>
        /// Returns the ApplicationId.
        /// </summary>
        /// <returns>The ApplicationId.</returns>
        string GetApplicationId();

        /// <summary>
        /// Returns the TransactionId.
        /// </summary>
        /// <returns>The TransactionId.</returns>
        string GetTransactionId();

        /// <summary>
        /// Returns the direct caller account name.
        /// </summary>
        /// <returns>The direct caller account name.</returns>
        string GetDirectCallerAccountName();

        /// <summary>
        /// Returns the original caller account name.
        /// </summary>
        /// <returns>The original caller account name.</returns>
        string GetOriginalCallerAccountName();
    }
}
