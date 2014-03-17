using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.EmailListener
{
    [TestClass]
    public class EmailConfigurationFixture : LoggingFixtureBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            LogFileReader.CreateDirectory("mail");
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        //Email TraceListener Name Empty
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenEmailTLNameEmpty()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration12"));
            factory.Create();
        }

        //Email TraceListener FromAddress Null
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenValidatingFromAddressEmpty()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration13"));
            //Passing when From Address is Empty
            factory.Create();
        }

        //Email TraceListener ToAddress Null
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownWhenToAddressEmpty()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration14"));
            factory.Create();
        }

        //Email TraceListener SMTP Port Null
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ExceptionIsThrownWhenSmtpPortEmpty()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration15"));
            factory.Create();
        }

        [TestMethod]
        public void EmailIsSentWhenCategoriesSeverityVerbose()
        {
            LogFileReader.CreateDirectory("mail");

            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration75"));
            this.writer = factory.Create();
            Logger.SetLogWriter(this.writer);

            LogEntry entry = new LogEntry();
            entry.Categories.Add("General");
            entry.Message = "Email Message Test";
            entry.EventId = 123;
            entry.Priority = 11;
            entry.Severity = TraceEventType.Error;

            this.writer.Write(entry);

            Assert.IsNotNull(LogFileReader.GetEmail());
        }

        [TestMethod]
        public void EmailIsSent()
        {
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection("loggingConfiguration100"));

            factory.Create().Write("Test", "General");

            Assert.IsNotNull(LogFileReader.GetEmail());
        }
    }
}
