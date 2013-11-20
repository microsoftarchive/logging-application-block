// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel;
using System.Configuration.Install;
using System.Management.Instrumentation;

[assembly: Instrumented(@"root\EnterpriseLibrary")]

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Tests
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : DefaultManagementProjectInstaller 
    {
        /// <summary>
        /// Represents the installer for the instrumentation events. Not intended for direct use.
        /// </summary>
        public ProjectInstaller()
        {
            ManagementInstaller managementInstaller = new ManagementInstaller();
            Installers.Add(managementInstaller);
        }
    }
}
