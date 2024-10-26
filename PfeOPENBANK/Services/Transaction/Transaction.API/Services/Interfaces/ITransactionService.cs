using Transaction.API.Models;
using Transaction.API.Models.Requests;
using Transaction.API.Models.Responses;

namespace Transaction.API.Services.Interfaces
{
    public interface ITransactionService
    {
        public Task<TransactionResponse> CreateTransactionRequest(string account_id, string bank_id, string type, TransactionRequestReq req);
        public Task<TransactionResponse> AnswerTransactionRequest(AnswerTransactionReq answer);
        public Task<TransactionRequestResponseList> GetTransactionsRequest(string account_id, string bank_id);
        public Task<TransactionResponseList> GetTransactionsForAccount(string account_id);
        public Task<TransactionResponse> GetTransactionAsync(string transactionId, string view_id);
    }
}
