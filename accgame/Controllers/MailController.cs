using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace accgame.Controllers
{
    public class MailController : Controller
    {
        // GET: Mail
        public ActionResult Index()
        {
            string toEmail = "Haidongok04@gmail.com";
            string subject = "Chu de....";
            string body = string.Format("Noi dung");

            EmailService service = new EmailService();
            bool kq = service.Send(toEmail, subject, body);
            return View();
        }
    }

    public class EmailService
    {
        public bool Send(string toEmail, string subject, string body)
        {
            using (var smtpClient = new SmtpClient())
            {
                var smtpUserName = "shopaccwibugamevn@gmail.com";
                var smtpPassword = "tnsukiaqdiqfwhmb";
                var smtpHost = "smtp.gmail.com";
                var smtpPort = 587;
                smtpClient.EnableSsl = true;
                smtpClient.Host = smtpHost;
                smtpClient.Port = smtpPort;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                var msg = new MailMessage
                {
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    From = new MailAddress(smtpUserName),
                    Subject = subject,
                    Body = body,
                    Priority = MailPriority.Normal,
                };
                msg.To.Add(toEmail);
                smtpClient.Send(msg);
                return true;
            }
        }
    }
}