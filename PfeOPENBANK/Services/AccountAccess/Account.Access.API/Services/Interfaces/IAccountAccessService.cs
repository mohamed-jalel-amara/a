using Account.Access.API.Models;
using Account.Access.API.Models.Requests;
using Account.Access.API.Models.Responses;

namespace Account.Access.API.Services.Interfaces
{
    public interface IAccountAccessService
    {
        public Task<AccountAccessResponse> CreateView(string Account_id, string bank_id, AddViewRequest request, string ownerId);
        public Task<AccountAccessResponse> UpdateView(string Account_id, string bank_id, int view_id, UpdateViewRequest request);
        public Task<AccountAccessResponse> GetOneAccessAsync(int view_id);
        public Task DeleteViewAsync(string Account_id, string bank_id, int view_id );
        public Task<AccountAccessResponse> GrantUserAccessToView(string provider, string 
            provider_id, int view_id);
        public Task<AccountAccessResponse> RevokeAccessToOneView(string provider, string
           provider_id, int view_id);
        public Task<UserAccessList> GetAccountAccessForUser(string provider, string
           provider_id);
        public Task<AccountAccessResponseList> GetAllViews();
    }
}
