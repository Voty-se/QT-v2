using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.Hosting;

namespace QT.Helpers
{
    public static class MailHandler
    {
        public static string SendEmail(string message, string to, string subject, bool attachement = false, string attachmentName = "LEVERANSVILLKOR Qtransport")
        {
            try
            {
                var mailMessage = new MailMessage("qtransport@voty.se", to)
                {
                    Body = message,
                    Subject = subject
                };

                if (attachement)
                    mailMessage.Attachments.Add(
                        new Attachment(HostingEnvironment.MapPath($@"~/Content/Statics/{attachmentName}.pdf")?? "", mediaType: MediaTypeNames.Application.Pdf));

                var client = new SmtpClient()
                {
                    Host = "send.one.com",
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential("qtransport@voty.se", "Str3355Xltz")
                };

                client.Send(mailMessage);

                return "";
            }
            catch (Exception e)
            {
                return $"{e.Message}. {e.InnerException?.Message}";
            }
        }
    }
}