// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Security;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
    /// <summary>
    /// Implementation of the <see cref="MsmqSendInterfaceFactory"/> contract that deals with an actual MSMQ.
    /// </summary>
    [SecurityCritical]
    public class MsmqSendInterfaceFactory : IMsmqSendInterfaceFactory
    {
        /// <summary>
        /// Returns a new instance of <see cref="MsmqSendInterface"/>
        /// </summary>
        /// <param name="queuePath">The MSMQ queue path.</param>
        /// <returns>The new MSMQ interface.</returns>
        [SecurityCritical]
        public IMsmqSendInterface CreateMsmqInterface(string queuePath)
        {
            return new MsmqSendInterface(queuePath);
        }
    }
}
