using EventBus.Message.Events;
using Identity.API.Applications.Dtos;
using Identity.API.Services.Interfaces;
using MassTransit;

namespace Identity.API.EventBusConsumer
{
    public class GrantEntitlementConsumer : IConsumer<GrantEntitlementEvent>
    {
        private readonly IEntitlementService _entitlementService;
        private readonly ILogger<GrantEntitlementConsumer> _logger;

        public GrantEntitlementConsumer(IEntitlementService entitlementService, ILogger<GrantEntitlementConsumer> logger)
        {
            _entitlementService = entitlementService ?? throw new ArgumentNullException(nameof(entitlementService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<GrantEntitlementEvent> context)
        {
            EntitlementRequest request = new EntitlementRequest() { Bank_Id = context.Message.BankId, Role_name = context.Message.EntitlementName };
            try
            {
                await _entitlementService.AddEntitlementForUserAsync(context.Message.UserId, request);
                _logger.LogInformation($"Grant Entitlement event consumed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogInformation($"An error has been occurred or the entitlement is already granted!.");
            }
        }
    }
}
