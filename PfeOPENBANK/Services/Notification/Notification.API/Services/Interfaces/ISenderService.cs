using Notification.API.Models;

namespace Notification.API.Services.Interfaces
{
    public interface ISenderService
    {
        public Task<SenderResponse> SendEmail(Email email);
        public Task<bool> SendSms(IdentityMessage message);
    }
}
