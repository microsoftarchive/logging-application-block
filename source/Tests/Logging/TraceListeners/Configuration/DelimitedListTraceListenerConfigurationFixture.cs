// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.TestSupport.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Tests.TraceListeners.Configuration
{
    [TestClass]
    public class DelimitedListTraceListenerConfigurationFixture
    {
        [TestInitialize]
        public void SetUp()
        {
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.CurrentDirectory);
            ConfigurationTestHelper.ConfigurationFileName = "test.exe.config";
        }

        private static TraceListener GetListener(string name, IConfigurationSource configurationSource)
        {
            var settings = LoggingSettings.GetLoggingSettings(configurationSource);
            return settings.TraceListeners.Get(name).BuildTraceListener(settings);
        }

        [TestMethod]
        public void CanCreateInstanceFromGivenName()
        {
            SystemDiagnosticsTraceListenerData listenerData
                = new SystemDiagnosticsTraceListenerData("listener", typeof(DelimitedListTraceListener), "log.txt");

            MockLogObjectsHelper helper = new MockLogObjectsHelper();
            helper.loggingSettings.TraceListeners.Add(listenerData);

            TraceListener listener = GetListener("listener", helper.configurationSource);

            Assert.IsNotNull(listener);
            Assert.AreEqual("listener", listener.Name);
            Assert.AreEqual(typeof(DelimitedListTraceListener), listener.GetType());
        }

        [TestMethod]
        public void CanCreateInstanceFromGivenConfigurationWithAttributes()
        {
            SystemDiagnosticsTraceListenerData listenerData
                = new SystemDiagnosticsTraceListenerData("listener", typeof(DelimitedListTraceListener), "log.txt");
            listenerData.SetAttributeValue("delimiter", "||");

            MockLogObjectsHelper helper = new MockLogObjectsHelper();
            helper.loggingSettings.TraceListeners.Add(listenerData);

            TraceListener listener = GetListener("listener", helper.configurationSource);

            Assert.IsNotNull(listener);
            Assert.AreEqual("listener", listener.Name);

            var innerListener = (DelimitedListTraceListener)listener;
            Assert.AreEqual("||", innerListener.Delimiter);
        }

        [TestMethod]
        public void CanCreateInstanceFromConfigurationFile()
        {
            SystemDiagnosticsTraceListenerData listenerData
                = new SystemDiagnosticsTraceListenerData("listener", typeof(DelimitedListTraceListener), "log.txt");
            listenerData.SetAttributeValue("delimiter", "||");
            LoggingSettings loggingSettings = new LoggingSettings();
            loggingSettings.TraceListeners.Add(listenerData);

            TraceListener listener = GetListener("listener", CommonUtil.SaveSectionsAndGetConfigurationSource(loggingSettings));

            Assert.IsNotNull(listener);
            Assert.AreEqual("listener", listener.Name);

            var innerListener = (DelimitedListTraceListener)listener;

            Assert.AreEqual("||", innerListener.Delimiter);
        }
    }
}
