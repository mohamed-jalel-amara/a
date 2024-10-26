using MediatR;
using Notification.API.Models;

namespace Notification.API.Features.Commands
{
    public class SendEmailCmd   : IRequest<SenderResponse>
    {
        public Email Emailrequest { get; set; }
    }
}
