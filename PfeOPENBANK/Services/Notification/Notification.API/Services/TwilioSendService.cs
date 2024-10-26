using Notification.API.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Notification.API.Services
{
    public class TwilioSenderService : Interfaces.ISenderService
    {
        public ILogger<TwilioSenderService> _logger { get; }
        private readonly IConfiguration _configuration;

        public TwilioSenderService() {

            TwilioClient.Init("ACa0f0902f10cfe371545d33297f603ac5", "43ff9488fe3e18c7054a21170ec4fb1f");
        }
        public Task<SenderResponse> SendEmail(Email email)
        {

            throw new NotImplementedException();
        }

        public Task<bool> SendSms(IdentityMessage message)
        {
            try
            {
                var mess = MessageResource.Create(
                        new PhoneNumber(message.Destination),
                        from: new PhoneNumber("+12489395096"),
                        body: message.Body
                 );
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                return Task.FromResult(false);
            }
        }
    }
}
