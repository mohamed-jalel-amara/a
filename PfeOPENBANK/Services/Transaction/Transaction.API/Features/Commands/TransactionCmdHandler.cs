using AutoMapper;
using MediatR;
using Transaction.API.Models;
using Transaction.API.Models.Responses;
using Transaction.API.Services.Interfaces;

namespace Transaction.API.Features.Commands
{
    public class TransactionCmdHandler : IRequestHandler<TransactionCmd, TransactionResponse>
    {
        private readonly ILogger<TransactionCmdHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITransactionService _service;

        public TransactionCmdHandler(ILogger<TransactionCmdHandler> logger, IMapper mapper, ITransactionService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TransactionResponse> Handle(TransactionCmd request, CancellationToken cancellationToken)
        {
            var transaction = await _service.CreateTransactionRequest(request.AccountId, request.BankId,
                request.Type, request.Request);

            return transaction;
        }
    }
}
