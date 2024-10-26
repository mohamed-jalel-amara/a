using View.grpc.Models;
using View.grpc.Models.Response;

namespace View.grpc.Services.Interfaces
{
    public interface IViewService
    {
        public Task<ViewModel> GetViewByIdAsync(int view_id);
        public Task<ViewAccessModel> GetUserViewAsync(string provider, string provider_id, int view_id);
        public Task<bool> GetIfUserHasCanAddTransactionRequestToAnyAccount(int view_id);
        public Task<ViewModelList> GetViewsForAccount(string accountId);
    }
}
