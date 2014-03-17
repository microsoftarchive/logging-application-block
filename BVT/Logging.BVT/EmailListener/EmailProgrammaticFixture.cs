using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.EmailListener
{
    [TestClass]
    public class EmailProgrammaticFixture : LoggingFixtureBase
    {
        private string fromaddress = "from@127.0.0.1.email.address";
        private string toaddress = "to@127.0.0.1.email.address";
        private string smtpserver = "localhost";
        private int smtpport = 0;
        private string userName = String.Empty;
        private string password = String.Empty;

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

        private void UpdateConfigForEmailNoPortFormatterEmailAuth(LoggingConfiguration loggingConfiguration)
        {
            if ((string.IsNullOrEmpty(this.fromaddress) || string.IsNullOrEmpty(this.toaddress) || string.IsNullOrEmpty(this.smtpserver)))
            {
                Assert.Inconclusive("Cannot run tests because SMTP server parameters are missing");
            }
            else
            {
                EmailTraceListener emailListener = new EmailTraceListener(this.toaddress,
                                                            this.fromaddress,
                                                            "StartOfSubject",
                                                            "EndOfSubject", this.smtpserver);

                emailListener.Filter = new EventTypeFilter(SourceLevels.All);
                loggingConfiguration.AddLogSource("Email", SourceLevels.All, true, emailListener);
                loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(emailListener);
            }
        }

        private void UpdateConfigForEmailNoPortEmailAuth(LoggingConfiguration loggingConfiguration)
        {
            if ((string.IsNullOrEmpty(this.fromaddress) || string.IsNullOrEmpty(this.toaddress) || string.IsNullOrEmpty(this.smtpserver)))
            {
                Assert.Inconclusive("Cannot run tests because SMTP server parameters are missing");
            }
            else
            {
                EmailTraceListener emailListener = new EmailTraceListener(this.toaddress,
                                                            this.fromaddress,
                                                            "StartOfSubject",
                                                            "EndOfSubject", this.smtpserver, new TextFormatter());

                emailListener.Filter = new EventTypeFilter(SourceLevels.All);
                loggingConfiguration.AddLogSource("Email", SourceLevels.All, true, emailListener);
                loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(emailListener);
            }
        }

        private void UpdateConfigForEmailNoFormatterEmailAuth(LoggingConfiguration loggingConfiguration)
        {
            if ((string.IsNullOrEmpty(this.fromaddress) || string.IsNullOrEmpty(this.toaddress) || string.IsNullOrEmpty(this.smtpserver)))
            {
                Assert.Inconclusive("Cannot run tests because SMTP server parameters are missing");
            }
            else
            {
                EmailTraceListener emailListener = new EmailTraceListener(this.toaddress,
                                                            this.fromaddress,
                                                            "StartOfSubject",
                                                            "EndOfSubject", this.smtpserver, this.smtpport);

                emailListener.Filter = new EventTypeFilter(SourceLevels.All);
                loggingConfiguration.AddLogSource("Email", SourceLevels.All, true, emailListener);
                loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(emailListener);
            }
        }

        private void UpdateConfigForEmailNoEmailAuth(LoggingConfiguration loggingConfiguration)
        {
            if ((string.IsNullOrEmpty(this.fromaddress) || string.IsNullOrEmpty(this.toaddress) || string.IsNullOrEmpty(this.smtpserver)))
            {
                Assert.Inconclusive("Cannot run tests because SMTP server parameters are missing");
            }
            else
            {
                EmailTraceListener emailListener = new EmailTraceListener(this.toaddress,
                                                            this.fromaddress,
                                                            "StartOfSubject",
                                                            "EndOfSubject", this.smtpserver, this.smtpport, new TextFormatter());

                emailListener.Filter = new EventTypeFilter(SourceLevels.All);
                loggingConfiguration.AddLogSource("Email", SourceLevels.All, true, emailListener);
                loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(emailListener);
            }
        }

        [TestMethod]
        public void EmailIsSentWhenGeneralCategoryAndNoPortFormatterEmailAuth()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEmailNoPortFormatterEmailAuth(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Log Entry in Email - EmailGeneralCategoryNoPortFormatterEmailAuth", "General",
                                        5, 9008, TraceEventType.Warning, "Logging Block EmailProgConfig Sample");
            string emailContent = LogFileReader.GetEmail();

            Assert.IsTrue(emailContent.Contains(string.Format("\r\nTo: {0}\r\n", this.toaddress)));
        }

        [TestMethod]
        public void EmailIsSentWhenGeneralCategoryNoPortEmailAuth()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEmailNoPortEmailAuth(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Log Entry in Email - EmailGeneralCategoryNoPortEmailAuth", "General",
                                        5, 9008, TraceEventType.Warning, "Logging Block EmailProgConfig Sample");
            string emailContent = LogFileReader.GetEmail();

            Assert.IsTrue(emailContent.Contains(string.Format("\r\nTo: {0}\r\n", this.toaddress)));
        }

        [TestMethod]
        public void EmailIsSentWhenGeneralCategoryNoFormatterEmailAuth()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEmailNoFormatterEmailAuth(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Log Entry in Email - EmailGeneralCategoryNoFormatterEmailAuth", "General",
                                        5, 9008, TraceEventType.Warning, "Logging Block EmailProgConfig Sample");
            string emailContent = LogFileReader.GetEmail();

            Assert.IsTrue(emailContent.Contains(string.Format("\r\nTo: {0}\r\n", this.toaddress)));
        }

        [TestMethod]
        public void EmailIsSentWhenGeneralCategoryNoEmailAuth()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEmailNoEmailAuth(loggingConfiguration);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Log Entry in Email - EmailGeneralCategoryNoEmailAuth", "General",
                                        5, 9008, TraceEventType.Warning, "Logging Block EmailProgConfig Sample");
            string emailContent = LogFileReader.GetEmail();

            Assert.IsTrue(emailContent.Contains(string.Format("\r\nTo: {0}\r\n", this.toaddress)));
        }

        [TestMethod]
        public void EmailIsNotSentWhenLogEnabledFilterIsFalse()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForEmailNoPortEmailAuth(loggingConfiguration);

            var logEnabledFilter = new LogEnabledFilter("LogEnabled Filter", false);
            loggingConfiguration.Filters.Add(logEnabledFilter);

            this.writer = new LogWriter(loggingConfiguration);
            this.writer.Write("Test Logging Not Present");
            this.writer.Dispose();

            string emailContent = LogFileReader.GetEmail();
            Assert.IsNull(emailContent);
        }
    }
}