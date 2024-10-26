namespace Transaction.API.Models.Responses
{
    public class TransactionRequestResponseList
    {
        public IList<TransactionRequestResponse> Transaction_requests_with_charges { get; set; } = new List<TransactionRequestResponse>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
