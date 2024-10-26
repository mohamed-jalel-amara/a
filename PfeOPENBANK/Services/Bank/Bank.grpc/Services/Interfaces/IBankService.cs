using Bank.grpc.Models;

namespace Bank.grpc.Services.Interfaces
{
    public interface IBankService
    {
       public Task<BankModel> GetBank(string bankId);
    }
}
