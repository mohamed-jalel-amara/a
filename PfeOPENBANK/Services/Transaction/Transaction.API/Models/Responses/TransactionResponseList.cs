namespace Transaction.API.Models.Responses
{
    public class TransactionResponseList
    {
        public IList<TransactionResponse> transactions { get; set; } = new List<TransactionResponse>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
