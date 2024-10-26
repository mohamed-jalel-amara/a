namespace Account.Access.API.Models.Responses
{
    public class AccountAccessList
    {
        public IList<AccountAccess> Items { get; set; } = new List<AccountAccess>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
