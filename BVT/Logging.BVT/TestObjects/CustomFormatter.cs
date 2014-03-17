using System.Collections.Specialized;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects
{
    [ConfigurationElementType(typeof(CustomFormatterData))]
    public class CustomFormatter : ILogFormatter
    {
        public CustomFormatter() { }

        public CustomFormatter(NameValueCollection collection)
        { }

        public bool FormattedInvoked = false;

        public string Format(LogEntry log)
        {
            FormattedInvoked = true;
            return "Formatted text.";
        }
    }
}