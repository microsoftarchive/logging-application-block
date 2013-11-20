// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
    /// <summary>
    /// Defines the behavior when the roll file is created.
    /// </summary>
    public enum RollFileExistsBehavior
    {
        /// <summary>
        /// Overwrites the file if it already exists.
        /// </summary>
        Overwrite,
        /// <summary>
        /// Use a secuence number at the end of the generated file if it already exists. If it fails again then increment the secuence until a non existent filename is found.
        /// </summary>
        Increment
    };
}
