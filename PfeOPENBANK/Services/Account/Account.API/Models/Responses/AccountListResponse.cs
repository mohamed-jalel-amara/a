namespace Account.API.Models.Responses
{
    public class AccountListResponse
    {
        public IList<AccountResponse> Accounts { get; set; } = new List<AccountResponse>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
