using Account.Grpc.Models;

namespace Account.Grpc.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<AccountModel> GetAccount(string id);
        public Task<AccountModelList> GetAccountsForAsync(string userId);
    }
}
