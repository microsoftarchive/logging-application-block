using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects
{
    public class MockEmailMessage : EmailMessage
    {
        private MailMessage lastMailMessageSent;

        public MockEmailMessage(EmailTraceListenerData emailParameters,
                                LogEntry logEntry,
                                ILogFormatter formatter)
            :
                base(emailParameters, logEntry, formatter) { }

        public MockEmailMessage(string toAddress,
                                string fromAddress,
                                string subjectLineStarter,
                                string subjectLineEnder,
                                string smtpServer,
                                int smtpPort,
                                string message,
                                ILogFormatter formatter)
            : base(toAddress, fromAddress, subjectLineStarter, subjectLineEnder, smtpServer, smtpPort, message, formatter) { }

        public override void Send()
        {
            this.lastMailMessageSent = this.CreateMailMessage();
        }
    }
}