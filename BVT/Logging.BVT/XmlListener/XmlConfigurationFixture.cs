using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.XmlListener
{
    [TestClass]
    public class XmlConfigurationFixture : LoggingFixtureBase
    {
        private const string XmlFilePath = "trace-xml.log";

        [TestInitialize]
        public override void TestInitialize()
       { 
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        private void LoadConfig(string configSection)
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(configSection));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);
        }

        [TestMethod]
        public void EntryIsWrittenWhenMaxXMLTraceListener()
        {
            this.LoadConfig("loggingConfiguration51");

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo(XmlFilePath);

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenEmptyXMLNameXMLTraceListener()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration52"));
        }

        [TestMethod]
        public void EntryIsWrittenWhenMinXMLTraceListener()
        {
            this.LoadConfig("loggingConfiguration50");

            var entry = LogEntryFactory.GetLogEntry();
            this.writer.Write(entry);

            FileInfo info = new FileInfo(XmlFilePath);

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void TraceListenerIsOfCorrectTypeWhenResolvingXmlTraceListener()
        {
            this.LoadConfig("loggingConfiguration50");

            var traceListener = writer.TraceSources.Values.SelectMany(x => x.Listeners).FirstOrDefault(l => l.Name == "XML Trace Listener");

            Assert.IsInstanceOfType(traceListener, typeof(XmlTraceListener));
        }

        [TestMethod]
        public void EntryIsWrittenWhenLoggingAnEntryUsingXmlTraceListener()
        {
            File.Delete(XmlFilePath);

            this.LoadConfig("loggingConfiguration50");

            var entry = LogEntryFactory.GetLogEntry();
            entry.Categories.Clear();
            entry.Categories.Add("General");

            this.writer.Write(entry);

            FileInfo info = new FileInfo(XmlFilePath);

            Assert.IsTrue(info.Exists);
            Assert.IsTrue(info.Length > 0);
        }

        [TestMethod]
        public void EntryIsWrittenWhenXmlLoggerWrites()
        {
            File.Delete(XmlFilePath);

            this.LoadConfig("loggingConfiguration50");

            LogEntry entry = LogEntryFactory.GetLogEntry();
            entry.Categories.Clear();
            entry.Categories.Add("General");

            Logger.Write(entry);

            var logEntryXDocument = LogFileReader.GetEntriesXml(XmlFilePath);

            string eventId = logEntryXDocument.Descendants(
                XName.Get("EventID", @"http://schemas.microsoft.com/2004/06/windows/eventlog/system"))
                    .First().Value;

            Assert.IsNotNull(eventId == entry.EventId.ToString());
        }
    }
}