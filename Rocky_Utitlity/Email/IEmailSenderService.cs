using System.Threading.Tasks;

namespace Rocky_Utility.Email
{
    public interface IEmailSenderService
    {
        Task<bool> SendEmailAsync(EmailDto item);
    }
}
