using View.grpc.Protos;

namespace Transaction.API.Services.Grpc
{
    public class ViewService
    {
        private readonly ViewProtoService.ViewProtoServiceClient _grpcClient;

        public ViewService(ViewProtoService.ViewProtoServiceClient grpcClient)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
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
    }
}
