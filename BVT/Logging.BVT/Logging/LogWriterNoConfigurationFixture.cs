using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.Logging
{
    [TestClass]
    public class LogWriterNoConfigurationFixture : EntLibFixtureBase
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExceptionIsThrownWhenNotConfigured()
        {
            Logger.Write("Test");
        }
    }
}
