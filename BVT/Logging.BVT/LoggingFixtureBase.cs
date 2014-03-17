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
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT
{
    public class LoggingFixtureBase : EntLibFixtureBase
    {
        protected const string EventLogName = "Application";
        protected const string EventLogNameCustom = "EntLib Tests";
        protected const string EventLogSourceName = "Enterprise Library Unit Tests";

        protected const string FailureMessage = "Expected exception was not thrown.";
        protected const string CategoryName = "Sample Category";

        protected ILoggingConfigurationStart configurationStart;
        protected ConfigurationSourceBuilder builder;
        protected LogWriter writer;
        protected string strPath;
        protected TextFormatter extendedFormatter;
        protected TextFormatter briefFormatter;
        protected string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        
        public LoggingFixtureBase()
            : base("FixtureConfiguration.config")
        { }

        public LoggingFixtureBase(string configSourceFileName)
            : base(configSourceFileName)
        { }

        protected EventLogEntry GetLastEventLogEntry()
        {
            EventLog events = new EventLog();
            events.Log = "Application";
            events.Source = "Enterprise Library Logging";

            return events.Entries[events.Entries.Count - 1];
//            return events.Entries.Cast<EventLogEntry>().Reverse().FirstOrDefault();
        }

        protected bool CheckForEntryInEventlog(string message)
        {
            EventLog events = new EventLog();
            events.Log = "Application";
            events.Source = "Enterprise Library Logging";

            EventLogEntry last = GetLastEventLogEntry();

            if (last.Message.Contains(message))
            {
                return true;
            }
            else
            {
                return events.Entries.Cast<EventLogEntry>().Any(e => e.Message.Contains(message));
            }
        }

        protected void ClearLogs()
        {
            //clear the log entries from the database
            try
            {
                using (var connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("ClearLogs", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "ClearLogs";
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqlException ex)
            {
                Assert.Inconclusive("Cannot access the database. " + Environment.NewLine + ex.Message);
            }
        }

        protected string GetLastLogMessage(string databaseName)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT TOP 1 FormattedMessage FROM Log ORDER BY TimeStamp DESC", connection);
                string messageContents = Convert.ToString(command.ExecuteScalar());
                connection.Close();
                return messageContents;
            }
        }

        protected void SetConfigurationSource()
        {
            DictionaryConfigurationSource source = new DictionaryConfigurationSource();
            this.builder.UpdateConfigurationWithReplace(source);
            this.ConfigurationSource = source;
        }

        protected LoggingSettings GetLoggingSettings()
        {
            DictionaryConfigurationSource configSource = new DictionaryConfigurationSource();
            this.builder.UpdateConfigurationWithReplace(configSource);

            return configSource.GetSection(LoggingSettings.SectionName) as LoggingSettings;
        }

        public virtual void TestInitialize()
        {
            this.builder = new ConfigurationSourceBuilder();
            this.configurationStart = this.builder.ConfigureLogging();
        }

        public override void Cleanup()
        {
            Logger.Reset();
            using (this.writer) { }

            if (this.strPath != null && Directory.Exists(this.strPath))
            {
                Directory.Delete(this.strPath, true);
            }
                        
            base.Cleanup();
        }

        protected LoggingConfiguration BuildProgrammaticConfigForTrace()
        {
            // Formatters
            this.briefFormatter = new TextFormatter("Timestamp: {timestamp(local)}{newline}Message: {message}{newline}Category: {category}{newline}Priority: {priority}{newline}EventId: {eventid}{newline}ActivityId: {property(ActivityId)}{newline}Severity: {severity}{newline}Title:{title}{newline}");
            this.extendedFormatter = new TextFormatter("Timestamp: {timestamp}{newline}Message: {message}{newline}Category: {category}{newline}Priority: {priority}{newline}EventId: {eventid}{newline}Severity: {severity}{newline}Title: {title}{newline}Activity ID: {property(ActivityId)}{newline}Machine: {localMachine}{newline}App Domain: {localAppDomain}{newline}ProcessId: {localProcessId}{newline}Process Name: {localProcessName}{newline}Thread Name: {threadName}{newline}Win32 ThreadId:{win32ThreadId}{newline}Extended Properties: {dictionary({key} - {value}{newline})}");

            // Build Configuration
            var config = new LoggingConfiguration();
            //config.AddLogSource("FlatFile", SourceLevels.All, true, flatFileTraceListener);

            // Other Settings
            config.IsTracingEnabled = true;
            config.DefaultSource = "General";
            config.LogWarningsWhenNoCategoriesMatch = true;

            return config;
        }

        protected LogWriter GetWriter(string fileName)
        {
            DictionaryConfigurationSource config = new DictionaryConfigurationSource();
            var logging = new LoggingSettings();

            logging.TracingEnabled = true;
            logging.Name = "Logging Application Block";
            logging.DefaultCategory = "General";
            logging.Formatters.Add(new BinaryLogFormatterData("Binary Formatter"));

            logging.TraceListeners.Add(
                new FlatFileTraceListenerData()
                {
                    Name = "TraceListener",
                    FileName = fileName,
                    TraceOutputOptions = TraceOptions.None,
                    Filter = SourceLevels.All,
                    Formatter = "Binary Formatter",
                    Type = typeof(FlatFileTraceListener)
                });

            var traceSource = new TraceSourceData("General", SourceLevels.Information, true);
            traceSource.TraceListeners.Add(new TraceListenerReferenceData("TraceListener"));

            logging.TraceSources.Add(traceSource);

            config.Add(LoggingSettings.SectionName, logging);

            this.ConfigurationSource = config;
            LogWriterFactory factory = new LogWriterFactory((e) => this.ConfigurationSource.GetSection(LoggingSettings.SectionName));
            return factory.Create();
        }
    }
}
