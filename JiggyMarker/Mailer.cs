using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;

namespace JiggyMarker
{
    internal class Mailer
    {
        SmtpClient Client;
        MailMessage Message;
        string JobNumber;
        ErrorHandler Errors;
        public Mailer(string jobNumber, ErrorHandler errorHandler)
        {
            Errors = errorHandler;
            Client = ConfigureSMTP();
            Message = ConfigureMessage();
            JobNumber = jobNumber;
        }
        public void SendMail(int SRLength, int JRLength, int comboLength, int srCount, int jrCount, int glueCount)
        {
            Message.Body = BuildMessage(SRLength, JRLength, comboLength, srCount, jrCount, glueCount);
            string[] ReportFiles = Directory.GetFiles(Path.Combine(Configurator.ReportDir, "ReportHold"));
            for(int i = 0; i < ReportFiles.Length; i++)
            {
                FileStream FS = new FileStream(ReportFiles[i], FileMode.Open, FileAccess.Read);
                ContentType CT = new ContentType(MediaTypeNames.Text.Csv);
                Message.Attachments.Add(new Attachment(FS, Path.GetFileName(ReportFiles[i]), "text/plain"));
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Client.Send(Message);
        }
        private SmtpClient ConfigureSMTP()
        {
            SmtpClient smtp = new SmtpClient("smtp.office365.com");
            smtp.TargetName = "STARTTLS/smtp.office365.com";
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(Configurator.MailAccount, Configurator.MailSecret);
            return smtp;
        }
        private MailMessage ConfigureMessage()
        {
            MailAddress from = new MailAddress(Configurator.MailAccount);
            MailAddress to = new MailAddress(Configurator.ReportEmail);
            //MailAddress to = new MailAddress("tim.owen@zenger.com");
            MailMessage message = new MailMessage(from, to);
            message.Subject = String.Format("Jiggy batch details for job {0} - {1}.", JobNumber, DateTime.Now.ToString("F"));
            message.IsBodyHtml = true;
            return message;
        }
        private string BuildMessage(int sr, int jr, int combo, int srCount, int jrCount, int glueCount)
        {
            int errCount = Errors.GetErrorCount();//ErrorCatcher.Errors.Count;
            int total = srCount + jrCount;
            string message = String.Format("Todays batch contains {0} total orders, with {1} total puzzles ready to print. There are {2} Glue Spreads in this batch. Please find attached batch report.", 
                (sr + jr + combo + errCount), (srCount + jrCount), glueCount) + Environment.NewLine;
            message += "<table><tr><th>SR Orders</th><th>JR Orders</th><th>Combo Orders</th><th>Errors</th><th>SR Puzzles</th><th>JR Puzzles</th><th>Total Puzzles</th></tr>";
            message += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr></table>", sr, jr, combo, errCount, srCount, jrCount, total);
            return message;
        }
    }
}
