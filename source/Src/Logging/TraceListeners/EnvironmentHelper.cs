// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Logging.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
    /// <summary>
    /// Helper class for working with environment variables.
    /// </summary>
    public class EnvironmentHelper
    {
        /// <summary>
        /// Sustitute the Environment Variables
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <returns></returns>
        public static string ReplaceEnvironmentVariables(string fileName)
        {
            // Check EnvironmentPermission for the ability to access the environment variables.
            try
            {
                string variables = Environment.ExpandEnvironmentVariables(fileName);

                // If an Environment Variable is not found then remove any invalid tokens
                Regex filter = new Regex("%(.*?)%", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                string filePath = filter.Replace(variables, "");

                if (Path.GetDirectoryName(filePath) == null)
                {
                    filePath = Path.GetFileName(filePath);
                }

                return filePath;
            }
            catch (SecurityException)
            {
                throw new InvalidOperationException(Resources.ExceptionReadEnvironmentVariablesDenied);
            }
        }
    }
}
