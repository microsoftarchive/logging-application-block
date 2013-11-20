// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Tests.Formatters
{
    [TestClass]
    public class ExceptionFormatterFixture
    {
        [TestMethod]
        public void SkippedNonReadableProperty()
        {
            ExceptionFormatter formatter = new ExceptionFormatter();

            Exception nonReadablePropertyException = new ExceptionWithNonReadableProperty("MyException");
            
            string message = formatter.GetMessage(nonReadablePropertyException);
            
            Assert.IsTrue(message.Length > 0);
        }

    }

    internal class ExceptionWithNonReadableProperty : Exception
    {
        public ExceptionWithNonReadableProperty(string message)
            : base(message)
        { }

        public string NonReadableProperty
        {
            set { ; }
        }
    }
}
