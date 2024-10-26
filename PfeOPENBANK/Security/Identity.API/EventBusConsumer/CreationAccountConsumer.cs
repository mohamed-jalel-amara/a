using EventBus.Message.Events;
using Identity.API.Applications.Models.Entities;
using Identity.API.Models;
using Identity.API.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.EventBusConsumer
{
    public class CreationAccountConsumer : IConsumer<CreationAccountEvent>
    {
        private readonly IPublishEndpoint _publish;
        private readonly UserManager<UserModel> _userManager;
        private readonly ILogger<AccountAccessConsumer> _logger;

        public CreationAccountConsumer(IPublishEndpoint publish, UserManager<UserModel> userManager, ILogger<AccountAccessConsumer> logger)
        {
            _publish = publish ?? throw new ArgumentNullException(nameof(publish));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<CreationAccountEvent> context)
        {
            var customer = await _userManager.FindByIdAsync(context.Message.OwnerId);
            if (customer != null)
            {
                SendEmailEvent email = new SendEmailEvent()
                {
                    To = customer.Email,
                    Body = "<p>You have just created a bank account. </p>" +
                           "" +
                           $"<p>Account Identifier: {context.Message.AccountId}  </p>" +
                           $"<p>Bank Identifier: { context.Message.BankId }</p>" +
                           $"<p>Created On: {DateTime.Now.ToLongDateString()} </p>",
                    Subject = "Confirmation of bank account creation"
                };
                await _publish.Publish(email);
                _logger.LogInformation($"Creation Account event consumed successfully.");
            }
        }
    }
}
