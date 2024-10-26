namespace Statement.API.Models.Responses
{
    public class AccountMinimal
    {
        public string Bank_id { get; set; }
        public long Account_number { get; set; }
        public string View_id { get; set; } = "owner";
        public IList<TransactionMinimal> Transactions { get; set; } = new List<TransactionMinimal>();
    }
}
