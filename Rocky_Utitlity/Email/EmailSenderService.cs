using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using System;
using Serilog;
using Rocky_Utility.Configuration.Models;

namespace Rocky_Utility.Email
{
    /// <summary>
    /// Email sender service
    /// Inject this class, and une SendEmailAsync for send an email
    /// </summary>
    public class EmailSenderService : IEmailSenderService
    {
        private readonly SmtpClient _smtpClient;
        private readonly EmailSettings _emailSettings;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="optionsAccessor"></param>
        public EmailSenderService(IOptions<EmailSettings> optionsAccessor)
        {
            _emailSettings = optionsAccessor.Value;
            _smtpClient = new SmtpClient(_emailSettings.Smtp.Host, _emailSettings.Smtp.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Smtp.Username, _emailSettings.Smtp.Password),
                EnableSsl = _emailSettings.Smtp.UseTls,
            };

            if (!_emailSettings.SendEmail)
            {
                var directory = new DirectoryInfo(_emailSettings.EmailFolder);

                if (!directory.Exists)
                    directory.Create();

                _smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                _smtpClient.PickupDirectoryLocation = _emailSettings.EmailFolder;
            }
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="item">EmailDto</param>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync(EmailDto item)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.DefaultSender),
                Body = item.Message,
                Subject = item.Subject,
                IsBodyHtml = true
            };
            mailMessage.To.Add(item.Addresses);

            if (!string.IsNullOrWhiteSpace(item.Cc))
                mailMessage.CC.Add(item.Cc);

            if (item.Attachments != null)
                foreach (var path in item.Attachments)
                    mailMessage.Attachments.Add(new Attachment(path));

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (ArgumentNullException)
            {
                Log.Error("Le destinataire ou l'expéditeur est manquant.");
            }
            catch (ArgumentException)
            {
                Log.Error("Le destinataire ou l'expéditeur est vide.");
            }
            catch (InvalidOperationException)
            {
                // This System.Net.Mail.SmtpClient has a Overload:System.Net.Mail.SmtpClient.SendAsync
                // call in progress. -or- System.Net.Mail.SmtpClient.DeliveryMethod property is
                // set to System.Net.Mail.SmtpDeliveryMethod.Network and System.Net.Mail.SmtpClient.Host
                // is null. -or- System.Net.Mail.SmtpClient.DeliveryMethod property is set to System.Net.Mail.SmtpDeliveryMethod.Network
                // and System.Net.Mail.SmtpClient.Host is equal to the empty string (""). -or- System.Net.Mail.SmtpClient.DeliveryMethod
                // property is set to System.Net.Mail.SmtpDeliveryMethod.Network and System.Net.Mail.SmtpClient.Port
                // is zero, a negative number, or greater than 65,535.
                Log.Error("Le serveur d'envoi de mail est surchargé.");
            }
            catch (SmtpException smtpException)
            {
                // The connection to the SMTP server failed. -or- Authentication failed. -or- The
                // operation timed out. -or- System.Net.Mail.SmtpClient.EnableSsl is set to true
                // but the System.Net.Mail.SmtpClient.DeliveryMethod property is set to System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory
                // or System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis. -or- System.Net.Mail.SmtpClient.EnableSsl
                // is set to true, but the SMTP mail server did not advertise STARTTLS in the response
                // to the EHLO command. -or- The message could not be delivered to one or more of
                // the recipients in recipients.
                Log.Error($"Le serveur mail distant a répondu une erreur de code: \"{(int)smtpException.StatusCode} {smtpException.StatusCode}\".");
            }

            return false;
        }

    }
}
