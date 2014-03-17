using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Logging
{
    [TestClass]
    public class LoggingFixture : LoggingFixtureBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration103"));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsThrownWhenLoggingANullEntry()
        {
            Logger.Write(null);
        }

        /// <summary>
        /// Verify that a non serializable collection can be serialized while logging with a binary formatter.
        /// Bug 18107
        /// </summary>
        [TestMethod]
        public void EntryIsWrittenWhenLoggingNonSerializableCollectionsWithTheBinaryFormatter()
        {
            var categories = new NonSerializableCollection();
            categories.Add("General");

            LogEntry logEntry = new LogEntry("message", categories, 1, 1, System.Diagnostics.TraceEventType.Resume, "title", null);
            this.writer.Write(logEntry);

            Assert.IsTrue(File.Exists("testingTrace.log"));
            Assert.IsTrue(File.Exists("testingTraceOther.log"));
            Assert.AreNotEqual(0, new FileInfo("testingTrace.log").Length); 
            Assert.AreNotEqual(0, new FileInfo("testingTraceOther.log").Length);
        }

        /// <summary>
        /// Verify that the XMLLogFormatter creates Valid XML for text contained in the properties.
        /// Bug 18145
        /// </summary>
        [TestMethod]
        public void EntryIsWrittenWithValidXmlWhenInvalidXMLCharactersAreUsedInProperties()
        {
            string xmlFile = "testingTrace-xml.log";
            File.Delete(xmlFile);

            var properties = new Dictionary<string, object>();
            properties.Add("invalid xml chars", @"!@#$%^&*(){}<>\/,./';[]\éàфыдлайцкшфывмьт");

            LogEntry logEntry = new LogEntry("message", "TestValidXML", 1, 1, System.Diagnostics.TraceEventType.Resume, "title", properties);
            this.writer.Write(logEntry);

            Assert.IsTrue(File.Exists(xmlFile));
            Assert.AreNotEqual(0, new FileInfo(xmlFile).Length);

            XDocument xmlDocument = LogFileReader.GetEntriesXml(xmlFile);
            Assert.IsTrue(xmlDocument.Nodes().Count() > 0);
        }

        //All Events Name Null
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenAllEventsNameEmpty()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration4"));
        }

        //Unprocessed Category name Null
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenUnprocessedCategoryNameEmpty()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration5"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExceptionIsThrownWhenEmptyTypeSysDiagnosticsTraceListener()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration55"));
            factory.Create();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenEmptyNameSysDiagnosticsTraceListener()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration56"));
        }
    }
}
