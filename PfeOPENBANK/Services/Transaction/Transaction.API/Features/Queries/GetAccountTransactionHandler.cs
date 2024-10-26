using MediatR;
using Transaction.API.Models.Responses;
using Transaction.API.Services.Interfaces;

namespace Transaction.API.Features.Queries
{
    public class GetAccountTransactionHandler : IRequestHandler<GetAccountTransactions, TransactionList>
    {
        private readonly ILogger<GetAccountTransactionHandler> _logger;
        private readonly ITransactionService _service;

        public GetAccountTransactionHandler(ILogger<GetAccountTransactionHandler> logger, ITransactionService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<TransactionList> Handle(GetAccountTransactions request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retrieving Transactions by Account :{request.Account_Number}");
            //var transactions = await _service.GetAllTransactionsAsync(request.Account_Number);

            return null;
        }
    }
}
