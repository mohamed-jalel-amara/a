using MediatR;
using Transaction.API.Models.Responses;

namespace Transaction.API.Features.Queries
{
    public class GetAccountTransactions : IRequest<TransactionList>
    {
        public long Account_Number { get; set; }
    }
}
