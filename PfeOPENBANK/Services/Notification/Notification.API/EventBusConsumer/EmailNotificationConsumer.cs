using AutoMapper;
using EventBus.Message.Events;
using MassTransit;
using MediatR;
using Notification.API.Features.Commands;
using Notification.API.Models;

namespace Notification.API.EventBusConsumer
{
    public class EmailNotificationConsumer : IConsumer<SendEmailEvent>
    {
        private readonly ILogger<EmailNotificationConsumer> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _sender;

        public EmailNotificationConsumer(ILogger<EmailNotificationConsumer> logger, IMapper mapper, IMediator sender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task Consume(ConsumeContext<SendEmailEvent> context)
        {
            _logger.LogInformation($"Sending email to {context.Message.To}...");
            var emailMessage = _mapper.Map<Email>(context.Message);
            var result = await _sender.Send(new SendEmailCmd() { Emailrequest = emailMessage});

            if (result.Status == true)
            {
                _logger.LogInformation($"Email sent to {context.Message.To}...");
            }
            else
            {
                _logger.LogError($"Email not sent to {context.Message.To}...");
            }
            
        }
    }
}
