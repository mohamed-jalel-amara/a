using Account.Grpc.Protos;

namespace Statement.API.Services.Grpc
{
    public class AccountService
    {
        private readonly AccountProtoService.AccountProtoServiceClient _grpcClient;

        public AccountService(AccountProtoService.AccountProtoServiceClient grpcClient)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
        }

        public async Task<AccountObject> GetAccountAsync(string id)
        {
            var getAccountRequest = new GetAccountRequest() { Id = id };
            return await _grpcClient.GetAccountAsync(getAccountRequest);
        }

        public async Task<AccountObjectList> GetAccountObjectListAsync(string userId)
        {
            var getAccountListRequest = new GetAccountsForUserRequest() { UserId = userId };
            return await _grpcClient.GetAccountsForUserAsync(getAccountListRequest);
        }
    }
}
