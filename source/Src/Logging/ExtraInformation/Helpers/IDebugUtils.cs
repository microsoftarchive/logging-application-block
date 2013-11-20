// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Helpers
{
    /// <summary>
    /// Contract for accessing debug information.
    /// </summary>
    public interface IDebugUtils
    {
        /// <summary>
        /// Returns a text representation of the stack trace with source information if available.
        /// </summary>
        /// <param name="stackTrace">The source to represent textually.</param>
        /// <returns>The textual representation of the stack.</returns>
        string GetStackTraceWithSourceInfo( StackTrace stackTrace );
    }
}
