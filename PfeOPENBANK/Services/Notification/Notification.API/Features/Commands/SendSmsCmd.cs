using MediatR;
using Notification.API.Models;

namespace Notification.API.Features.Commands
{
    public class SendSmsCmd : IRequest<bool>
    {
        public IdentityMessage SmsMessage { get; set; }
    }
}
