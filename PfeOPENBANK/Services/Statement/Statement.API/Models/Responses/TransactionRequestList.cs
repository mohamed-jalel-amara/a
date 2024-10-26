namespace Statement.API.Models.Responses
{
    public class TransactionRequestList
    {
        public IList<TransactionRequest> Items { get; set; } = new List<TransactionRequest>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
