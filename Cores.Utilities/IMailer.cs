using Cores.Models;

namespace Cores.Utilities;

public interface IMailer
{
    Task SendEmailAsync(EmailPayload emailData);
}   