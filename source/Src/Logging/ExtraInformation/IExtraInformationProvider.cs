// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation
{
    /// <summary>
    /// Defines a method to populate an <see cref="IDictionary{K,T}"/> with helpful diagnostic information.
    /// </summary>
    public interface IExtraInformationProvider
    {
        /// <summary>
        /// Populates an <see cref="IDictionary{K,T}"/> with helpful diagnostic information.
        /// </summary>
        /// <param name="dict">Dictionary containing extra information used to initialize the <see cref="IExtraInformationProvider"></see> instance</param>
        void PopulateDictionary(IDictionary<string, object> dict);
    }
}
