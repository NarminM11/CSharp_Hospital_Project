using System.Net.Mail;
using System.Net;
using UserNamespace;

namespace EmailhelperNamespace
{
    public  class EmailHelper
    {
        public List<User> _users { get; set; }
        public static void SendEmailToAdmin(string subject, string body, string email)
        {
            var fromAddress = new MailAddress("nrmin.mrdova.01@bk.ru", "Mərkəzi Sağlamlıq Klinikası");
            var toAddress = new MailAddress(email, "Admin");
            const string fromPassword = "lWocXqoQsO9kOSOhEmj9";

            var smtp = new SmtpClient
            {
                Host = "smtp.mail.ru",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };


            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }

}