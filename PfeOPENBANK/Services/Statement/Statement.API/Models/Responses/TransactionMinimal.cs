namespace Statement.API.Models.Responses
{
    public class TransactionMinimal
    {
        public DateTime Date { get; set; }
        public string Transaction_Id { get; set; }
        public string Description { get; set; }
        public string? Type { get; set; }
        public TransactionValue Value { get; set; }
    }
}
