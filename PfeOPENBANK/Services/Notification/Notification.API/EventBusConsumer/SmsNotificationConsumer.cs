using AutoMapper;
using EventBus.Message.Events;
using MassTransit;
using MediatR;
using Notification.API.Features.Commands;
using Notification.API.Models;

namespace Notification.API.EventBusConsumer
{
    public class SmsNotificationConsumer : IConsumer<SendSmsEvent>
    {
        private readonly ILogger<SmsNotificationConsumer> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _sender;

        public SmsNotificationConsumer(ILogger<SmsNotificationConsumer> logger, IMapper mapper, IMediator sender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }
        public async Task Consume(ConsumeContext<SendSmsEvent> context)
        {
            _logger.LogInformation($"Sending sms to {context.Message.Destination}...");
            var smsMessage = _mapper.Map<IdentityMessage>(context.Message);
            var success = await _sender.Send(new SendSmsCmd() { SmsMessage = smsMessage});

            if (success)
            {
                _logger.LogInformation($"Sms sent to {context.Message.Destination}...");
            }
        }
    }
}
