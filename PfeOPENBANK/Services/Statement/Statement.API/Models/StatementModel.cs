using Statement.API.Models.Responses;

namespace Statement.API.Models
{
    public class StatementModel
    {
        public IList<AccountMinimal> Accounts { get; set; } = new List<AccountMinimal>();
    }
}
