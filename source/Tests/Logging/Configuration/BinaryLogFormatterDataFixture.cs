// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Tests.Configuration
{
    [TestClass]
    public class GivenBinaryLogFormatterTypeRegistrationEntry
    {
        private BinaryLogFormatterData formatterData;

        [TestInitialize]
        public void Given()
        {
            this.formatterData = new BinaryLogFormatterData("formatterName");
        }

        [TestMethod]
        public void when_creating_formatter_then_creates_binary_formatter()
        {
            var formatter = (BinaryLogFormatter)this.formatterData.BuildFormatter();

            Assert.IsNotNull(formatter);
        }
    }
}
