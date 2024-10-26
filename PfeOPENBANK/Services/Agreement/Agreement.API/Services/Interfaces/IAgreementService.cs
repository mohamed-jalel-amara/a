using Agreement.API.Models;
using Agreement.API.Models.Requests;
using Agreement.API.Models.Responses;

namespace Agreement.API.Services.Interfaces
{
    public interface IAgreementService
    {
        public Task<AccountAccessList> GetAllAccessListAsync();
        public Task<AccountAccessList> GetAllAccessListAsync(string userId);
        public Task<AccountAccessList> GetAccessListByAccountAsync(string userId, long account_number);
        public Task<AccountAccessList> GetAccessListByProviderAsync(string userId, string providerId);
        public Task<AccountAccess> GetOneAccessAsync(int accessId);
        public Task<bool> EditAccessAsync(AccessRequest request, string userId);
        public Task<bool> UpdateAccessAsync(AccountAccess access);
    }
}
