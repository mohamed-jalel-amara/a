namespace Transaction.API.Models.Responses
{
    public class TransactionDetail
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Posted { get; set; }
        public DateTime Completed { get; set; }
        public TransactionValue New_balance { get; set; }
        public TransactionValue Value { get; set; }
    }
}
