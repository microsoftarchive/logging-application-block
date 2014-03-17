using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects
{
    [ConfigurationElementType(typeof(CustomTraceListenerData))]
    public class MyCustomTraceListener : CustomTraceListener
    {
        public static bool Wrote = false;
        public static bool WroteLine = false;

        public override void Write(string message)
        {
            if (this.Formatter != null)
            {
                this.Formatter.Format(new LogEntry() { Message = message });
            }

            Wrote = true;
        }

        public override void WriteLine(string message)
        {
            if (this.Formatter != null)
            {
                this.Formatter.Format(new LogEntry() { Message = message });
            }

            WroteLine = true;
        }
    }
}