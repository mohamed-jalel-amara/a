using MediatR;
using Transaction.API.Models.Requests;
using Transaction.API.Models.Responses;

namespace Transaction.API.Features.Commands
{
    public class TransactionCmd : IRequest<TransactionResponse>
    {
        public string AccountId { get; set; }
        public string BankId { get; set; }
        public string Type { get; set; } 
        public TransactionRequestReq Request { get; set; }
    }
}
