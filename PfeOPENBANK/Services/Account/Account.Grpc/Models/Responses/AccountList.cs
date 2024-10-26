namespace Account.Grpc.Models.Responses
{
    public class AccountList
    {
        public IList<AccountModel> Items { get; set; } = new List<AccountModel>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
        public IList<AccountLinks> Links { get; set; } = new List<AccountLinks>();
    }
}
