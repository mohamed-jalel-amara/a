using Account.Grpc.Protos;

namespace Transaction.API.Services.Grpc
{
    public class AccountService
    {
        private readonly AccountProtoService.AccountProtoServiceClient _grpcClient;

        public AccountService(AccountProtoService.AccountProtoServiceClient grpcClient)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
        }

        public async Task<AccountObject> GetAccountDataAsync(string accountId)
        {
            var accountRequest  = new GetAccountRequest() { Id = accountId };
            return await _grpcClient.GetAccountAsync(accountRequest);
        }
    }
}
