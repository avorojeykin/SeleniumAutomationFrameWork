using ModuleTester.Models;
using System;
using System.Net.Mail;
using Common.Models;
using Common.Utilities;

namespace ModuleTester.Services
{
    public class EmailService
    {
        public static string SendEmail(EmailSettings emailSettings, string recipient, ProcessResponseEmail emailDetails )
        {
            string result = "ok";
            try
            {
                SmtpClient smtpClient = new SmtpClient(emailSettings.SmtpHost);
                smtpClient.Port = emailSettings.Port;
                smtpClient.UseDefaultCredentials = true;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(emailSettings.Sender);
                mail.To.Add(new MailAddress(recipient));
                mail.Subject = emailDetails.Subject;
                mail.Body = $"{emailDetails.Header}\r\n{emailDetails.Body}\r\n{emailDetails.Trailer}";
                smtpClient.Send(mail);
                return result;
            }
            catch (Exception ex)
            {
                return CommonUtilities.GetExceptionString(ref ex);
            }
        }
    }
}
