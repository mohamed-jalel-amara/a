using Account.API.Models;
using Account.API.Services.Interfaces;
using AutoMapper;
using EventBus.Message.Events;
using MassTransit;

namespace Account.API.EventBusConsumer
{
    public class TransactionConsumer : IConsumer<AccountEvent>
    {
        private readonly ILogger<TransactionConsumer> _logger;
        private readonly IMapper _mapper;
        private readonly IAccountService _service;

        public TransactionConsumer(ILogger<TransactionConsumer> logger, IMapper mapper, IAccountService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task Consume(ConsumeContext<AccountEvent> context)
        {
            var account = _mapper.Map<AccountModel>(context.Message);
            await _service.UpdateAccount(account, context.Message.TRANSACTIONEVENTID);
            _logger.LogInformation($"AccountEvent consumed successfully. Updated Account Id : {account.AccNumber}", account);
        }
    }
}
