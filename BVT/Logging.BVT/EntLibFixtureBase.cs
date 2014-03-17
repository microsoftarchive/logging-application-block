using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT
{
    public class EntLibFixtureBase 
    {
        private IConfigurationSource source;

        public EntLibFixtureBase()
            : this(null, false, Assembly.GetCallingAssembly())
        {
        }

        public EntLibFixtureBase(string configSourceFileName)
            : this(configSourceFileName, false, Assembly.GetCallingAssembly())
        {
        }

        public EntLibFixtureBase(string configSourceFileName, bool useMultipleSources, Assembly resourceAssembly)
        {
            ResourceAssembly = resourceAssembly;

            ConfigurationSourceFileName = configSourceFileName;

            UseMultipleSources = useMultipleSources;
            ConfigurationSource = DoSetup();
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            if (ConfigurationSource != null)
            {
                ConfigurationSource.Dispose();
            }
        }

        protected string ConfigurationSourceFileName { get; private set; }

        protected Assembly ResourceAssembly { get; private set; }

        protected bool UseMultipleSources { get; private set; }

        protected IConfigurationSource ConfigurationSource
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }

        protected virtual IConfigurationSource DoSetup()
        {
            if (source == null)
            {
                if (ConfigurationSourceFileName == null)
                {
                    source = ConfigurationSourceFactory.Create();
                }
                else
                {
                    WriteEmbeddedFileToDisk(ResourceAssembly, ConfigurationSourceFileName);

                    if (!UseMultipleSources)
                    {
                        source = new FileConfigurationSource(ConfigurationSourceFileName);
                    }
                    else
                    {
                        ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                        fileMap.ExeConfigFilename = ConfigurationSourceFileName;

                        System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                        // Added support for multiple sources
                        ConfigurationSourceSection configurationSourceSection
                            = configuration.GetSection(ConfigurationSourceSection.SectionName) as ConfigurationSourceSection;

                        if (configurationSourceSection != null)
                        {
                            string systemSourceName = configurationSourceSection.SelectedSource;
                            if (!string.IsNullOrEmpty(systemSourceName))
                            {
                                ConfigurationSourceElement objectConfiguration
                                    = configurationSourceSection.Sources.Get(systemSourceName);

                                source = objectConfiguration.CreateSource();
                            }
                        }
                    }
                }
            }

            return source;
        }

        public static void WriteEmbeddedFileToDisk(Assembly assembly, string fileName)
        {
            WriteEmbeddedFileToDisk(assembly, fileName, fileName);
        }

        public static void WriteEmbeddedFileToDisk(Assembly assembly, string sourceFileName, string targetFileName)
        {
            //if file exists, delete it or it will be old...
            if (File.Exists(targetFileName))
            {
                File.Delete(targetFileName);
            }

            using (var stream = GetEmbeddedFileStream(assembly, sourceFileName))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                File.WriteAllBytes(targetFileName, bytes);
            }
        }

        public static Stream GetEmbeddedFileStream(Assembly assembly, string fileName)
        {
            string assemblyName = assembly.GetName().Name;

            string resourceName = assemblyName + "." + fileName;

            var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new ApplicationException(string.Format("Unable to find embedded resource: {0}", resourceName));
            }

            return stream;
        }
    }
}
