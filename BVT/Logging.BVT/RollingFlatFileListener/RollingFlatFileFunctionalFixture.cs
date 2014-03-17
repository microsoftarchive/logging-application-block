using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.RollingFlatFileListener
{
    [TestClass]
    public class RollingFlatFileFunctionalFixture
    {
        private MockDateTimeProvider dateTimeProvider = new MockDateTimeProvider();
        private TextFormatter textFormatter;
        private string fileNameWithoutExtension;
        private string fileName;
        private const string Extension = ".log";

        [TestInitialize]
        public void SetUp()
        {
            textFormatter = new TextFormatter("----Template----");
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.CurrentDirectory);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (fileName != null)
            {
                foreach (string createdFileName in Directory.GetFiles(".", fileNameWithoutExtension + "*"))
                {
                    File.Delete(createdFileName);
                }
            }
        }

        [TestMethod]
        public void FileIsRolledAndArchivedWhenIntervalSetToMidnightAndDayIncremented()
        {
            fileNameWithoutExtension = Guid.NewGuid().ToString();
            fileName = fileNameWithoutExtension + Extension;

            using (RollingFlatFileTraceListener traceListener
                  = new RollingFlatFileTraceListener(fileName, "--header--", "--footer--", null,
                                                     0, "yyyy", RollFileExistsBehavior.Increment, RollInterval.Midnight))
            {
                traceListener.TraceData(new TraceEventCache(),
                                        "source",
                                        TraceEventType.Information,
                                        0,
                                        "logged message 1");
                traceListener.RollingHelper.DateTimeProvider = dateTimeProvider;

                dateTimeProvider.SetCurrentDateTime(DateTime.Now);
                Assert.IsTrue(traceListener.RollingHelper.UpdateRollingInformationIfNecessary());

                dateTimeProvider.SetCurrentDateTime(DateTime.Now.AddDays(1).Date);
                //dateTimeProvider.currentDateTime = DateTime.Now.AddMinutes(2);
                Assert.IsTrue(traceListener.RollingHelper.UpdateRollingInformationIfNecessary());

                traceListener.TraceData(new TraceEventCache(), "source", TraceEventType.Information, 1, "logged message 2");
            }

            Assert.IsTrue(File.Exists(fileName));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(fileName).Contains("logged message 2"));

            string[] archiveFiles = Directory.GetFiles(".", fileNameWithoutExtension + "*");
            Assert.AreEqual(2, archiveFiles.Length);
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(fileNameWithoutExtension + "." + DateTime.Today.Year + ".1" + Extension).Contains("logged message 1"));
        }

        [TestMethod]
        public void FileIsRolledWhenIntervalSetToMidnightAndOverwrite()
        {
            fileNameWithoutExtension = Guid.NewGuid().ToString();
            fileName = fileNameWithoutExtension + Extension;

            using (RollingFlatFileTraceListener traceListener
                  = new RollingFlatFileTraceListener(fileName, "--header--", "--footer--", null,
                                                     0, String.Empty, RollFileExistsBehavior.Overwrite, RollInterval.Midnight))
            {
                traceListener.TraceData(new TraceEventCache(),
                                        "source",
                                        TraceEventType.Information,
                                        0,
                                        "logged message 1");
                traceListener.RollingHelper.DateTimeProvider = dateTimeProvider;

                dateTimeProvider.SetCurrentDateTime(DateTime.Now);
                Assert.IsTrue(traceListener.RollingHelper.UpdateRollingInformationIfNecessary());

                dateTimeProvider.SetCurrentDateTime(DateTime.Now.AddDays(1).Date);
                //dateTimeProvider.currentDateTime = DateTime.Now.AddMinutes(2);
                Assert.IsTrue(traceListener.RollingHelper.UpdateRollingInformationIfNecessary());

                traceListener.TraceData(new TraceEventCache(), "source", TraceEventType.Information, 1, "logged message 2");
            }

            Assert.IsTrue(File.Exists(fileName));
            Assert.IsTrue(LogFileReader.ReadFileWithoutLock(fileName).Contains("logged message 2"));
            Assert.IsFalse(LogFileReader.ReadFileWithoutLock(fileName).Contains("logged message 1"));

            string[] archiveFiles = Directory.GetFiles(".", fileNameWithoutExtension + "*");

            Assert.AreEqual(1, archiveFiles.Length);
        }
    }
}