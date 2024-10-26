using Bank.api.Models.Requests;
using Bank.api.Models.Responses;

namespace Bank.api.Services.Interfaces
{
    public interface IBankServices
    {
        public Task<BankResponse> CreateBankAsync(BankRequest request);
        public Task<BankResponseList> GetAllBankAsync();
        public Task<BankResponse> GetBankAsync(string id);
        public Task<bool> DeleteBankAsync(string id);
    }
}
