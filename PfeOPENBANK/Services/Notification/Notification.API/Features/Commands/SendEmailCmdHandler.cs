using MediatR;
using Notification.API.Models;
using Notification.API.Services.Interfaces;

namespace Notification.API.Features.Commands
{
    public class SendEmailCmdHandler : IRequestHandler<SendEmailCmd, SenderResponse>
    {
        private readonly ISenderService _sender;
        public SendEmailCmdHandler(ISenderService sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task<SenderResponse> Handle(SendEmailCmd request, CancellationToken cancellationToken)
        {
            return await _sender.SendEmail(request.Emailrequest);
        }
    }
}
