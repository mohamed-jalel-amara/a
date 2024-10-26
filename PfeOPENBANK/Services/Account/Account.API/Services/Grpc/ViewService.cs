using Account.API.Models;
using AutoMapper;
using View.grpc.Protos;

namespace Account.API.Services.Grpc
{
    public class ViewService
    {
        private readonly ViewProtoService.ViewProtoServiceClient _grpcClient;
        private readonly IMapper _mapper;

        public ViewService(ViewProtoService.ViewProtoServiceClient grpcClient, IMapper mapper)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ViewObject> GetViewDataAsync(int view_id)
        {
            var viewRequest = new GetViewRequest() { Id = view_id };
            return await _grpcClient.GetViewAsync(viewRequest);
        }

        public async Task<ViewAccess> GetViewAccessDataAsync(string provider, string provider_id,int view_id)
        {
            var accessRequest = new GetUserViewRequest() { ViewId = view_id, Provider = provider, ProviderId = provider_id };
            return await _grpcClient.GetUserViewAsync(accessRequest);
        }

        public async Task<ViewModelList> GetViewsForAccount(string accountId)
        {
            var viewsRequest = new GetViewsForAccountRequest() { AccountId = accountId };
            var result = await _grpcClient.GetViewsForAccountAsync(viewsRequest);
            return _mapper.Map<ViewModelList>(result);
        }
    }
}
