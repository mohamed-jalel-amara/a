using MediatR;
using Notification.API.Services.Interfaces;

namespace Notification.API.Features.Commands
{
    public class SendSmsCmdHandler : IRequestHandler<SendSmsCmd, bool>
    {
        private readonly ISenderService _sender;
        public SendSmsCmdHandler(ISenderService sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }
        public Task<bool> Handle(SendSmsCmd request, CancellationToken cancellationToken)
        {
            return _sender.SendSms(request.SmsMessage);
        }
    }
}
