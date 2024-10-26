using MediatR;
using Transaction.API.Models;
using Transaction.API.Models.Responses;

namespace Transaction.API.Features.Queries
{
    public class GetTransactionRequestsCmd : IRequest<TransactionRequestResponseList>
    {
        public string AccountId { get; set; }
        public string BankId { get; set; }
    }
}
