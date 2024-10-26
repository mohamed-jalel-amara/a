using Branch.API.Models;
using Branch.API.Models.Requests;
using Branch.API.Models.Response;
using System.Linq.Expressions;

namespace Branch.API.Services.Interfaces
{
    public interface IBranchService
    {
        Task<BranchResponse> AddBranch(BranchRequest branch);
        Task<bool> DeleteBranch(string branch_id);
        Task<BranchListResponse> GetAllBranches(string bank_id,int? page, int? size);
        Task<BranchListResponse> GetBranchesByFilter(string filter,string bank_id, int? page, int? size);
        Task<BranchResponse> GetBranch(string id);
    }
}
