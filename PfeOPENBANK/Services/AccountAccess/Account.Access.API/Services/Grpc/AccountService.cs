using Account.Grpc.Protos;

namespace Account.Access.API.Services.Grpc
{
    public class AccountService
    {
        private readonly AccountProtoService.AccountProtoServiceClient _grpcClient;

        public AccountService(AccountProtoService.AccountProtoServiceClient grpcClient)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
        }

        public async Task<AccountObject> GetAccountDataAsync(string id)
        {
            var accountRequest = new GetAccountRequest() { Id = id};
            return await _grpcClient.GetAccountAsync(accountRequest);
        }
    }
}
