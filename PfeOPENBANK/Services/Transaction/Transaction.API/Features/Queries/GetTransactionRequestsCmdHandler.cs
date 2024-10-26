using MediatR;
using Transaction.API.Models;
using Transaction.API.Models.Responses;
using Transaction.API.Services.Interfaces;

namespace Transaction.API.Features.Queries
{
    public class GetTransactionRequestsCmdHandler : IRequestHandler<GetTransactionRequestsCmd, TransactionRequestResponseList>
    {
        private readonly ILogger<GetTransactionRequestsCmdHandler> _logger;
        private readonly ITransactionService _service;

        public GetTransactionRequestsCmdHandler(ILogger<GetTransactionRequestsCmdHandler> logger, ITransactionService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<TransactionRequestResponseList> Handle(GetTransactionRequestsCmd request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving of transaction  requests...");
            var transactions = await _service.GetTransactionsRequest(request.AccountId, request.BankId);

            return transactions;
        }
    }
}
