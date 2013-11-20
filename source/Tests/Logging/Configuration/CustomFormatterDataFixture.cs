// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Tests.Configuration
{
    [TestClass]
    public class GivenCustomerFormatterDataRegistry
    {
        private CustomFormatterData formatterData;

        [TestInitialize]
        public void Given()
        {
            this.formatterData = new CustomFormatterData("myName", typeof(MockCustomLogFormatter));
            this.formatterData.Attributes.Add(MockCustomLogFormatter.AttributeKey, "bar");
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void when_creating_formatter_then_creates_custom_formatter()
        {
            var formatter = (MockCustomLogFormatter)this.formatterData.BuildFormatter();
            Assert.AreEqual("bar", formatter.customValue);
        }
    }
}
