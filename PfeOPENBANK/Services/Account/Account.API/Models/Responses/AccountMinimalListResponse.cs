namespace Account.API.Models.Responses
{
    public class AccountMinimalListResponse
    {
        public IList<AccountMinimal> Accounts { get; set; } = new List<AccountMinimal>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
