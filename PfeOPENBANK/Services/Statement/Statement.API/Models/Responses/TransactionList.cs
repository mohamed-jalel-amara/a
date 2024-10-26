namespace Statement.API.Models.Responses
{
    public class TransactionList
    {
        public IList<TransactionModel> Items { get; set; } = new List<TransactionModel>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
        //public IList<TransactionLinks> Links { get; set; } = new List<TransactionLinks>();
    }
}
