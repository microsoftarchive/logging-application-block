// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Messaging;
using System.Security;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
    /// <summary>
    /// Specifies the contract for a MSMQ interface object.
    /// </summary>
    [SecurityCritical]
    public interface IMsmqSendInterface : IDisposable
    {
        /// <summary>
        /// Close the msmq.
        /// </summary>
        void Close();

        /// <summary>
        /// Send a message to the MSMQ.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> to send.</param>
        /// <param name="transactionType">The <see cref="MessageQueueTransactionType"/> value that specifies the type of transaction to use.</param>
        void Send(Message message, MessageQueueTransactionType transactionType);

        /// <summary>
        /// The transactional status of the MSMQ.
        /// </summary>
        bool Transactional { get; }
    }
}
