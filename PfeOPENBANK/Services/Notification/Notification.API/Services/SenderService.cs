using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Net;
using Notification.API.Models;
using Notification.API.Services.Interfaces;
using Twilio.Clients;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;
using static ASPSMSX2.ASPSMSX2SoapClient;
using Twilio.Types;

namespace Notification.API.Services
{
    public class SenderService : ISenderService
    {
        public EmailSettings _emailSettings { get; }
        public ILogger<SenderService> _logger { get; }
        private readonly IConfiguration _configuration;

        public SenderService(IOptions<EmailSettings> emailSettings, IConfiguration configuration,
            ILogger<SenderService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _configuration = configuration;
            TwilioClient.Init("AC0760b7f5d1015d28b4808dacc741cc85", "593a3233a2b58dd37b7fd15ae3b94c13");
        }

        public async Task<SenderResponse> SendEmail(Email email)
        {
            //var client = new SendGridClient(_configuration.GetValue<string>("EmailSettings:ApiKey"));

            //var subject = email.Subject;
            //var to = new EmailAddress(email.To);
            //var emailBody = email.Body;

            //var from = new EmailAddress
            //{
            //    Email = _configuration.GetValue<string>("EmailSettings:FromAddress"),
            //    Name = _configuration.GetValue<string>("EmailSettings:FromName")
            //};

            //var sendGridMessage = MailHelper.CreateSingleEmail(from, to, subject, emailBody, emailBody);
            //var response = await client.SendEmailAsync(sendGridMessage);

            //_logger.LogInformation("Email sent.");

            //if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
            //    return new EmailResponse() { Status = true, Message = "Email sent."};

            //_logger.LogError("Email sending failed.");
            //return new EmailResponse() { Status = false, Message = "Email sending failed."};
            string fromMail = _configuration.GetValue<string>("EmailSettings:Smtp:FromEmail");
            string fromPassword = _configuration.GetValue<string>("EmailSettings:Smtp:Pwd");

            if (email != null)
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = email.Subject;
                message.To.Add(new MailAddress(email.To));
                message.Body = "<html><body> " + email.Body + " </body></html>";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };
                smtpClient.Send(message);
                return new SenderResponse() { Status = true, Message = "Email sent." };
            }
            return new SenderResponse() { Status = false, Message = "Email not sent." };
        }

        public async Task<bool> SendSms(IdentityMessage message )
        {
            bool status = true;
            try
            {
                if (message != null)
                {
                    try
                    {
                        var mess = MessageResource.Create(
                        new PhoneNumber(message.Destination),
                        from: new PhoneNumber("+18588425325"),
                        body: message.Body
                 );
                    }
                    catch
                    {
                        await SendEmail(new Email() { Body = message.Body, Subject = message.Subject, To = message.ToEmail });
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Send sms failed! "+ex.Message);
                status = false;
            }
            return status;
        }
    }
}
