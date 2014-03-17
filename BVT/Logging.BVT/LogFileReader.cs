using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Newtonsoft.Json;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.BVT
{
    public static class LogFileReader
    {
        private static string SanitizeStringForDeserialization(string content)
        {
            // Remove Header & Footer
            content = content.Replace("-", String.Empty);
            // Remove invalid characters
            content = content.Replace("=", String.Empty);
            content = content.Replace("\r\n", String.Empty);
            content = content.Replace(" ", "+");

            int additionalCharacters = content.Length % 4;
            if (additionalCharacters > 0)
            {
                content += new string('=', 4 - additionalCharacters);
            }

            return content;
        }

        public static LogEntry GetEntry(string fileName)
        {
            string serializedContent = ReadFileWithoutLock(fileName);

            serializedContent = SanitizeStringForDeserialization(serializedContent);

            return BinaryLogFormatter.Deserialize(serializedContent);
        }

        public static XDocument GetEntriesXml(string fileName)
        {
            XDocument doc = null;

            using (var reader = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                doc = XDocument.Load(reader);
            }

            return doc;
        }

        public static string ReadFileWithoutLock(string fileName)
        {
            using (var reader = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                return reader.ReadToEnd();
            }
        }

        public static void CreateDirectory(string strPath)
        {
            if (Directory.Exists(strPath))
            {
                Directory.Delete(strPath, true);
            }
            Directory.CreateDirectory(strPath);
        }

        public static string GetEmail()
        {
            string emailContent = null;

            if (Directory.Exists("mail"))
            {
                string fileName = Directory.GetFiles("mail", "*.eml").FirstOrDefault();

                if (fileName != null)
                {
                    emailContent = ReadFileWithoutLock(fileName);
                }
            }

            return emailContent;
        }

        public static string GetEmailBody()
        {
            string emailContent = LogFileReader.GetEmail();

            string endOfHeader = "\r\n\r\n";
            int index = emailContent.IndexOf(endOfHeader);
            string base64 = emailContent.Substring(index + endOfHeader.Length, emailContent.Length - index - endOfHeader.Length);
            string text = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(base64));

            return text;
        }

        public static LogEntry GetLogEntryFromEmail()
        {
            string jsonText = GetEmailBody();

            LogEntry logEntry = JsonLogFormatter.Deserialize<LogEntry>(jsonText);

            return logEntry;
        }
    }
}
