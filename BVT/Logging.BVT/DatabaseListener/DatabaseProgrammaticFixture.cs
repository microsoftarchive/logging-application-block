using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Database;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.DatabaseListener
{
    [TestClass]
    public class DatabaseProgrammaticFixture : LoggingFixtureBase
    {
        private const string BinaryFileName = "staticFacadeTrace.log";
        private const int DefaultPriority = -1;
        private const TraceEventType DefaultSeverity = TraceEventType.Information;
        private const int DefaultEventId = 1;
        private const string DefaultTitle = "";
        private static readonly string WrongConnectionString = ConfigurationManager.ConnectionStrings["wrongConnectionString"].ConnectionString;
        
        [TestInitialize]
        public override void TestInitialize()
        {
            this.ClearLogs();

            DatabaseProviderFactory factory = new DatabaseProviderFactory(new SystemConfigurationSource(false));
            DatabaseFactory.SetDatabaseProviderFactory(factory, false);
        }

        [TestCleanup]
        public override void Cleanup()
        {
            DatabaseFactory.ClearDatabaseProviderFactory();

            base.Cleanup();
        }

        private void UpdateConfigForDatabaseTL(LoggingConfiguration loggingConfiguration)
        {
            SqlDatabase dbConnection = new SqlDatabase(connectionString);

            var databaseTraceListener = new FormattedDatabaseTraceListener(dbConnection, "WriteLog", "AddCategory", briefFormatter);

            loggingConfiguration.AddLogSource("Database", SourceLevels.All, true, databaseTraceListener);
            loggingConfiguration.SpecialSources.Unprocessed.Listeners.Add(databaseTraceListener);
        }

        private string CheckEntryInDatabase()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT TOP 1 FormattedMessage FROM Log ORDER BY TimeStamp DESC", connection);
                string messageContents = Convert.ToString(command.ExecuteScalar());
                connection.Close();
                return messageContents;
            }
        }

        private int CheckEntryInDatabase(string strMessage)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select COUNT(1) from Log where Message = '" + strMessage + "'", connection);
                int messageContents = (Int32)command.ExecuteScalar();
                connection.Close();
                return messageContents;
            }
        }

        [TestMethod]
        public void EntryIsWrittenToDatabase()
        {
            var config = new LoggingConfiguration();
            config.IsTracingEnabled = true;
            config.DefaultSource = "General";
            config.LogWarningsWhenNoCategoriesMatch = true;

            SqlDatabase dbConnection = new SqlDatabase(connectionString);

            var databaseTraceListener = new FormattedDatabaseTraceListener(dbConnection, "WriteLog", "AddCategory", briefFormatter);
            config.SpecialSources.Unprocessed.Listeners.Add(databaseTraceListener);

            this.writer = new LogWriter(config);

            this.writer.Write("Test Log Entry in Database", "Database",
                                                    2, 1001, TraceEventType.Warning, "Logging Block ProgConfig Sample", null);
            string dateTimeLogged = DateTime.Now.ToLocalTime().ToString();

            string strMessage = this.CheckEntryInDatabase();

            Assert.IsTrue(strMessage.Contains("Test Log Entry in Database"));
        }

        [TestMethod]
        public void EntryIsWrittenToDatabaseWhenGeneralCategory()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForDatabaseTL(loggingConfiguration);
            this.writer = new LogWriter(loggingConfiguration);

            this.writer.Write("Test Log Entry in Database", "Database",
                                                    2, 1001, TraceEventType.Warning, "Logging Block ProgConfig Sample", null);
            string dateTimeLogged = DateTime.Now.ToLocalTime().ToString();

            string strMessage = this.CheckEntryInDatabase();

            Assert.IsTrue(strMessage.Contains("Timestamp: " + dateTimeLogged + "\r\nMessage: Test Log Entry in Database\r\nCategory: Database\r\nPriority: 2\r\nEventId: 1001"));
            Assert.IsTrue(strMessage.Contains("Severity: Warning\r\nTitle:Logging Block ProgConfig Sample"));
        }

        [TestMethod]
        public void EntryIsNotWrittenToDatabaseWhenLogEnabledFilterIsFalse()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForDatabaseTL(loggingConfiguration);
            var logEnabledFilter = new LogEnabledFilter("LogEnabled Filter", false);
            loggingConfiguration.Filters.Add(logEnabledFilter);

            this.writer = new LogWriter(loggingConfiguration);

            this.writer.Write("Invalid Log Entry. LogEnabledFilter is False", "Database",
                                                    2, 1001, TraceEventType.Warning, "Logging Block ProgConfig Sample", null);
            string dateTimeLogged = DateTime.Now.ToLocalTime().ToString();

            string strMessage = this.CheckEntryInDatabase();

            Assert.IsFalse(strMessage.Contains("Timestamp: " + dateTimeLogged + "\r\nMessage: Invalid Log Entry. LogEnabledFilter is False"));
        }

        [TestMethod]
        public void EntriesAreOnlyWrittenToDatabaseWhenPassPriorityFilter()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForDatabaseTL(loggingConfiguration);
            var priorityFilter = new PriorityFilter("Priority Filter", 2, 99);
            loggingConfiguration.Filters.Add(priorityFilter);
            this.writer = new LogWriter(loggingConfiguration);

            this.writer.Write("Test Logging PriorityFilter 1", "General", 1);
            this.writer.Write("Test Logging PriorityFilter 2", "General", 2);
            this.writer.Write("Test Logging PriorityFilter 101", "General", 101);

            string dateTimeLogged = DateTime.Now.ToLocalTime().ToString();

            int checkPriority1 = this.CheckEntryInDatabase("Test Logging PriorityFilter 1");
            int checkPriority101 = this.CheckEntryInDatabase("Test Logging PriorityFilter 101");

            Assert.AreEqual(0, checkPriority1);
            Assert.AreEqual(0, checkPriority101);

            string strMessage = this.CheckEntryInDatabase();

            Assert.IsTrue(strMessage.Contains("Timestamp: " + dateTimeLogged + "\r\nMessage: Test Logging PriorityFilter 2\r\nCategory: General\r\nPriority: 2\r\nEventId: 1"));
        }

        [TestMethod]
        public void EntryIsOnlyWrittenToDatabaseWhenPassesCategoryFilter()
        {
            LoggingConfiguration loggingConfiguration = BuildProgrammaticConfigForTrace();
            this.UpdateConfigForDatabaseTL(loggingConfiguration);

            // Category Filters
            ICollection<string> categories = new List<string>();
            categories.Add("BlockedByFilter");
            var categoryFilter = new CategoryFilter("Category Filter", categories, CategoryFilterMode.DenyAllExceptAllowed);
            loggingConfiguration.Filters.Add(categoryFilter);

            this.writer = new LogWriter(loggingConfiguration);

            this.writer.Write("Test Logging CategoryFilter 1", "General", 1);
            this.writer.Write("Test Logging CategoryFilter 2", "BlockedByFilter", 2);

            string dateTimeLogged = DateTime.Now.ToLocalTime().ToString();

            int checkCategory1 = this.CheckEntryInDatabase("Test Logging CategoryFilter 1");
            Assert.AreEqual(0, checkCategory1);

            string strMessage = this.CheckEntryInDatabase();

            Assert.IsTrue(strMessage.Contains("Timestamp: " + dateTimeLogged + "\r\nMessage: Test Logging CategoryFilter 2\r\nCategory: BlockedByFilter\r\nPriority: 2\r\nEventId: 1"));
        }

        [TestMethod]
        public void EntryIsWrittenToDatabaseWhenSeverityIsWarning()
        {
            Data.Database dbProvider = new DatabaseProviderFactory(this.ConfigurationSource).Create("Connection String");
            Assert.IsNotNull(dbProvider);
            FormattedDatabaseTraceListener listener = new FormattedDatabaseTraceListener(dbProvider,
                                                                                         "WriteLog",
                                                                                         "AddCategory",
                                                                                         new TextFormatter("DBTL TEST{newline}DBTL TEST"));

            listener.TraceData(new TraceEventCache(), "source", TraceEventType.Error, 0, new LogEntry("Message-Warning",
                                                                                                        "cat1",
                                                                                                        0,
                                                                                                        0,
                                                                                                        TraceEventType.Warning,
                                                                                                        "title",
                                                                                                        null));
            string result = GetLastLogMessage("Connection String");

            Assert.AreNotEqual(0, result.Length);
            Assert.AreEqual("DBTL TEST" + Environment.NewLine + "DBTL TEST", result);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void ExceptionIsThrownWhenWrongConnectionString()
        {
            FormattedDatabaseTraceListener listener = new FormattedDatabaseTraceListener(new SqlDatabase(WrongConnectionString), "WriteLog", "AddCategory", new TextFormatter("TEST{newline}TEST"));
            listener.TraceData(new TraceEventCache(), "source", TraceEventType.Error, 0, new LogEntry("message", "cat1", 0, 0, TraceEventType.Error, "title", null));
        }

        [TestMethod]
        public void EntryIsWrittenToDatabaseWhenSpecialCategoriesErrorMax()
        {
            Data.Database dbProvider = new DatabaseProviderFactory(this.ConfigurationSource).Create("Connection String");
            Assert.IsNotNull(dbProvider);
            FormattedDatabaseTraceListener listener = new FormattedDatabaseTraceListener(dbProvider,
                                                                                         "WriteLog",
                                                                                         "AddCategory",
                                                                                         new TextFormatter("DBTL TEST{newline}DBTL TEST"));

            LogEntry entry = new LogEntry();
            entry.Message = "Message 1";
            entry.Categories.Add("General");
            entry.EventId = 123;
            entry.Priority = 11;
            entry.Severity = TraceEventType.Error;
            entry.Title = "Db Trace listener title";
            entry.Win32ThreadId = "Win32ThreadId";
            entry.TimeStamp = DateTime.Now;
            entry.ProcessId = "100";
            entry.ProcessName = "Process Name";
            entry.MachineName = "CHNSHL123456";
            entry.ActivityId = Guid.NewGuid();

            listener.TraceData(new TraceEventCache(), "source", TraceEventType.Error, 0, entry);
            string result = GetLastLogMessage("Connection String");

            Assert.AreNotEqual(0, result.Length);
            Assert.AreEqual("DBTL TEST" + Environment.NewLine + "DBTL TEST", result);
        }

        [TestMethod]
        public void EntryIsWrittenToDatabaseWhenSeverityIsError()
        {
            Data.Database dbProvider = new DatabaseProviderFactory(this.ConfigurationSource).Create("Connection String");
            Assert.IsNotNull(dbProvider);
            FormattedDatabaseTraceListener listener = new FormattedDatabaseTraceListener(dbProvider,
                                                                                         "WriteLog",
                                                                                         "AddCategory",
                                                                                         new TextFormatter("DBTL TEST{newline}DBTL TEST"));
            listener.Filter = new EventTypeFilter(SourceLevels.Information);

            listener.TraceOutputOptions = TraceOptions.Callstack;
            listener.TraceOutputOptions = TraceOptions.DateTime;
            listener.TraceOutputOptions = TraceOptions.LogicalOperationStack;
            listener.TraceOutputOptions = TraceOptions.None;
            listener.TraceOutputOptions = TraceOptions.ThreadId;
            listener.TraceOutputOptions = TraceOptions.Timestamp;

            LogEntry entry = new LogEntry();
            entry.Message = "Message 1";
            entry.Categories.Add("Log Errors and Warnings");
            entry.EventId = 123;
            entry.Priority = 11;
            entry.Severity = TraceEventType.Error;
            entry.Title = "Db Trace listener title2";
            entry.Win32ThreadId = "Win32ThreadId";
            entry.TimeStamp = DateTime.Now;
            entry.ProcessId = "100";
            entry.ProcessName = "Process Name";
            entry.MachineName = "CHNSHL123456";
            entry.ActivityId = Guid.NewGuid();

            listener.TraceData(new TraceEventCache(), "source", TraceEventType.Error, 0, entry);
            string result = GetLastLogMessage("Connection String");
            Assert.AreNotEqual(0, result.Length);
            Assert.AreEqual("DBTL TEST" + Environment.NewLine + "DBTL TEST", result);
        }
    }
}