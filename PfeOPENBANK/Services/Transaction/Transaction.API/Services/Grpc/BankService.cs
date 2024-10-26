using Bank.grpc.Protos;

namespace Transaction.API.Services.Grpc
{
    public class BankService
    {
        private readonly BankProtoService.BankProtoServiceClient _grpcClient;

        public BankService(BankProtoService.BankProtoServiceClient grpcClient)
        {
            _grpcClient = grpcClient ?? throw new ArgumentNullException(nameof(grpcClient));
        }

        public async Task<BankObject> GetBankDataAsync(string bankId)
        {
            var bankRequest = new GetBankRequest() { Id = bankId };
            return await _grpcClient.GetBankAsync(bankRequest);
        }
    }
}
