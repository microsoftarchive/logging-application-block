// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Security;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
    /// <summary>
    /// Specifies the contract for a provider of MSMQ interfaces.
    /// </summary>
    [SecurityCritical]
    public interface IMsmqSendInterfaceFactory
    {
        /// <summary>
        /// Returns a new MSMQ interface.
        /// </summary>
        /// <param name="queuePath">The MSMQ queue path.</param>
        /// <returns>The new MSMQ interface.</returns>
        IMsmqSendInterface CreateMsmqInterface(string queuePath);
    }
}
