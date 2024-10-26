using Branch.GRPC.Models;
using Branch.GRPC.Models.Response;
using System.Linq.Expressions;

namespace Branch.GRPC.Services.Interfaces
{
    public interface IBranchService
    {
        Task<BranchList> GetBranchesByFilter(string filter);
        Task<BranchModel> GetBranch(int id);
        Task<BranchModel> GetOneBranchByName(string name);
    }
}
