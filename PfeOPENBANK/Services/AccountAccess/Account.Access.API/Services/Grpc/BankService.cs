using Bank.grpc.Protos;

namespace Account.Access.API.Services.Grpc
{
    public class BankService
    {
        private readonly BankProtoService.BankProtoServiceClient _grpcClient;

        public BankService(BankProtoService.BankProtoServiceClient grpcClient)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
        }
        public async Task<BankObject> GetBankAsync(string bankId)
        {
            var bankReq = new GetBankRequest() { Id = bankId };
            return await _grpcClient.GetBankAsync(bankReq);
        }
    }
}
