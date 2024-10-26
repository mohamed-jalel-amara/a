using Statement.API.Models;
using Statement.API.Models.Responses;

namespace Statement.API.Services.Interfaces
{
    public interface IStatementService
    {
        public Task<StatementModel> GetStatementAsync(string userId);
    }
}
