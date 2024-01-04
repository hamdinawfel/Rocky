using System.Threading.Tasks;

namespace Rocky.Utils.Email
{
    public interface IEmailSenderService
    {
        Task<bool> SendEmailAsync(EmailDto item);
    }
}
