using Microsoft.AspNetCore.Identity.UI.Services;
using Cores.Models;
using Cores.Utilities;

public class EmailSenderAdapter : IEmailSender
{
    private readonly IMailer _mailer;

    public EmailSenderAdapter(IMailer mailer)
    {
        _mailer = mailer;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailPayload = new EmailPayload
        {
            Email = email,
            Subject = subject,
            Message = htmlMessage
        };

        return _mailer.SendEmailAsync(emailPayload);
    }
}