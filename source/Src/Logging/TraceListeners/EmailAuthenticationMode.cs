// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
    /// <summary>
    /// This enumeration defines the options that the <see cref="EmailTraceListener"/>
    /// can use to authenticate to the STMP server.
    /// </summary>
    public enum EmailAuthenticationMode
    {
        /// <summary>
        /// No authentication
        /// </summary>
        None = 0,

        /// <summary>
        /// Use the Windows credentials for the current process
        /// </summary>
        WindowsCredentials,

        /// <summary>
        /// Pass a user name and password
        /// </summary>
        UserNameAndPassword
    }
}
