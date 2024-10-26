namespace Account.Grpc.Models
{
    public class AccountModelList
    {
        public IList<AccountModel> Items { get; set; } = new List<AccountModel>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
