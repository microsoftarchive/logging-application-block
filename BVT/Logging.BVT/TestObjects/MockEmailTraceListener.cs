using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT.TestObjects
{
    public class MockEmailTraceListener : EmailTraceListener
    {
        private const string MockFromAddress = "logging@entlib.com";
        private const int MockSmtpPort = 25;
        private const string MockSmtpServer = "smtphost";
        private const string MockSubjectLineEnder = "has occurred";
        private const string MockSubjectLineStarter = "EntLib-Logging:";
        private const string MockToAddress = "obviously.bad.email.address@127.0.0.1;another.not.so.good.email.address@127.0.0.1";

        private EmailTraceListenerData emailData;
        private int numberMessagesSent = 0;

        public MockEmailTraceListener(ILogFormatter formatter)
            : this(MockToAddress, MockFromAddress, MockSubjectLineStarter, MockSubjectLineEnder, MockSmtpServer, MockSmtpPort, formatter) { }

        public MockEmailTraceListener(string toAddress,
                                      string fromAddress,
                                      string subjectLineStarter,
                                      string subjectLineEnder,
                                      string smtpServer)
            : base(toAddress, fromAddress, subjectLineStarter, subjectLineEnder, smtpServer)
        {
            this.emailData = new EmailTraceListenerData(toAddress, fromAddress, subjectLineStarter, subjectLineEnder, smtpServer, string.Empty);
        }

        public MockEmailTraceListener(string toAddress,
                                      string fromAddress,
                                      string subjectLineStarter,
                                      string subjectLineEnder,
                                      string smtpServer,
                                      int smtpPort)
            : base(toAddress, fromAddress, subjectLineStarter, subjectLineEnder, smtpServer, smtpPort)
        {
            this.emailData = new EmailTraceListenerData(toAddress, fromAddress, subjectLineStarter, subjectLineEnder, smtpServer, smtpPort, string.Empty);
        }

        public MockEmailTraceListener(string toAddress,
                                      string fromAddress,
                                      string subjectLineStarter,
                                      string subjectLineEnder,
                                      string smtpServer,
                                      ILogFormatter formatter)
            : base(toAddress, fromAddress, subjectLineStarter, subjectLineEnder, smtpServer, formatter)
        {
            this.emailData = new EmailTraceListenerData(toAddress, fromAddress, subjectLineStarter, subjectLineEnder, smtpServer, string.Empty);
        }

        public MockEmailTraceListener(string toAddress,
                                      string fromAddress,
                                      string subjectLineStarter,
                                      string subjectLineEnder,
                                      string smtpServer,
                                      int smtpPort,
                                      ILogFormatter formatter)
            : base(toAddress, fromAddress, subjectLineStarter, subjectLineEnder, smtpServer, smtpPort, formatter)
        {
            this.emailData = new EmailTraceListenerData(toAddress, fromAddress, subjectLineStarter, subjectLineEnder, smtpServer, smtpPort, string.Empty);
        }

        public int MessagesSent
        {
            get { return this.numberMessagesSent; }
        }

        public override void TraceData(TraceEventCache eventCache,
                                       string source,
                                       TraceEventType eventType,
                                       int id,
                                       object data)
        {
            EmailMessage message = null;
            if (data is LogEntry)
            {
                message = new MockEmailMessage(this.emailData, data as LogEntry, Formatter);
                message.Send();
                this.numberMessagesSent++;
            }
            else if (data is string)
            {
                this.Write(data);
            }
            else
            {
                base.TraceData(eventCache, source, eventType, id, data);
            }
        }

        public override void Write(string message)
        {
            MockEmailMessage mailMessage = new MockEmailMessage(this.emailData.ToAddress, 
                this.emailData.FromAddress, 
                this.emailData.SubjectLineStarter, 
                this.emailData.SubjectLineEnder, 
                this.emailData.SmtpServer,
                this.emailData.SmtpPort, 
                message, 
                this.Formatter);
            
            mailMessage.Send();
            this.numberMessagesSent++;
        }
    }
}
