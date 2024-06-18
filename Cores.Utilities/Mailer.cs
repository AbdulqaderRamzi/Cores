using System.Net;
using System.Net.Mail;
using Cores.Models;

namespace Cores.Utilities;

public class Mailer : IMailer
{
    public Task SendEmailAsync(EmailPayload emailData) {
        var client = new SmtpClient("smtp.office365.com.", 587) {   
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("aboodg9@hotmail.com", "Mr3b59_99KG")
        };
 
        return client.SendMailAsync(
            new MailMessage(from: "aboodg9@hotmail.com",
                to: emailData.Email,
                emailData.Subject,
                emailData.Message
            ));
    }
}