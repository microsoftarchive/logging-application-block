using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.XmlListener
{
    [TestClass]
    public class XmlFluentConfigurationFixture : LoggingFixtureBase
    {
        private new const string CategoryName = "Debug";

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        [TestMethod]
        public void EntryIsWrittenWhenLoggingWithXmlFile()
        {
            const string FilePath = "sample.xml";
            const string ListenerName = "Xml File Listener";

            File.Delete(FilePath);

            configurationStart.LogToCategoryNamed(CategoryName)
                            .WithOptions
                            .SetAsDefaultCategory()
                            .ToSourceLevels(SourceLevels.All)
                            .SendTo
                                .XmlFile(ListenerName)
                                .ToFile(FilePath)
                                .WithTraceOptions(TraceOptions.None);

            this.SetConfigurationSource();

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(e));

            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            LogEntry entry = LogEntryFactory.GetLogEntry();
            entry.Categories.Add(CategoryName);

            writer.Write(entry);

            FileInfo info = new FileInfo(FilePath);

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }
    }
}