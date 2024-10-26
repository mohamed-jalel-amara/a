using EventBus.Message.Events;
using MassTransit;
using System.Security.Principal;
using Transaction.API.Models;
using Transaction.API.Services.Interfaces;

namespace Transaction.API.EventBusConsumer
{
    public class AccountConsumer : IConsumer<TransactionEvent>
    {
        private readonly ILogger<AccountConsumer> _logger;
        private readonly ITransactionService _service;

        public AccountConsumer(ILogger<AccountConsumer> logger, ITransactionService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task Consume(ConsumeContext<TransactionEvent> context)
        {
            //TransactionModel transaction = new TransactionModel() { STATUS = context.Message.STATUS, 
            //    TRANSID = context.Message.TRANSID}; 

            //await _service.UpdateTransactionAsync(transaction);
            //_logger.LogInformation($"TransactionEvent consumed successfully. Updated Transaction Id : {transaction.TRANSID}", transaction);
        }
    }
}
