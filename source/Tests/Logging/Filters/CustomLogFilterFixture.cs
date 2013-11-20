// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.TestSupport.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Filters.Tests
{
    [TestClass]
    public class CustomLogFilterFixture
    {
        [TestInitialize]
        public void TestInitialize()
        {
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.CurrentDirectory);
        }

        private static ILogFilter GetFilter(string name, IConfigurationSource configurationSource)
        {
            var settings = LoggingSettings.GetLoggingSettings(configurationSource);
            return settings.LogFilters.Get(name).BuildFilter();
        }

        [TestMethod]
        public void CanBuildCustomLogFilterFromGivenConfiguration()
        {
            CustomLogFilterData filterData
                = new CustomLogFilterData("custom", typeof(MockCustomLogFilter));
            filterData.SetAttributeValue(MockCustomProviderBase.AttributeKey, "value1");

            MockLogObjectsHelper helper = new MockLogObjectsHelper();
            helper.loggingSettings.LogFilters.Add(filterData);

            ILogFilter filter = GetFilter(filterData.Name, helper.configurationSource);

            Assert.IsNotNull(filter);
            Assert.AreSame(typeof(MockCustomLogFilter), filter.GetType());
            Assert.AreEqual("value1", ((MockCustomLogFilter)filter).customValue);
        }

        [TestMethod]
        public void CanDeserializeSerializedConfiguration()
        {
            LoggingSettings rwLoggingSettings = new LoggingSettings();
            rwLoggingSettings.LogFilters.Add(new CustomLogFilterData("filter1", typeof(MockCustomLogFilter)));

            IDictionary<string, ConfigurationSection> sections = new Dictionary<string, ConfigurationSection>();
            sections[LoggingSettings.SectionName] = rwLoggingSettings;
            IConfigurationSource configurationSource
                = ConfigurationTestHelper.SaveSectionsInFileAndReturnConfigurationSource(sections);

            LoggingSettings roLoggingSettings = (LoggingSettings)configurationSource.GetSection(LoggingSettings.SectionName);

            Assert.AreEqual(1, roLoggingSettings.LogFilters.Count);
            Assert.IsNotNull(roLoggingSettings.LogFilters.Get("filter1"));
            Assert.AreSame(typeof(CustomLogFilterData), roLoggingSettings.LogFilters.Get("filter1").GetType());
            Assert.AreSame(typeof(MockCustomLogFilter), roLoggingSettings.LogFilters.Get("filter1").Type);
        }
    }
}
