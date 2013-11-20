// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Tests.Configuration
{
    [TestClass]
    public class GivenTextFormatterDataSection
    {
        private TextFormatterData formatterData;

        [TestInitialize]
        public void Given()
        {
            this.formatterData = new TextFormatterData("formatterName", "someTemplate");
        }

        [TestMethod]
        public void when_creating_formatter_then_creates_text_formatter()
        {
            var formatter = (TextFormatter)this.formatterData.BuildFormatter();

            Assert.AreEqual("someTemplate", formatter.Template);
        }
    }
}
