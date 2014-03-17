using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects
{
    public class MockDateTimeProvider : RollingFlatFileTraceListener.DateTimeProvider
    {
        private DateTime? currentDateTime = null;

        public override DateTime CurrentDateTime
        {
            get
            {
                return this.currentDateTime ?? base.CurrentDateTime;
            }
        }

        public void SetCurrentDateTime(DateTime currentDateTime)
        {
            this.currentDateTime = currentDateTime;
        }
    }
}