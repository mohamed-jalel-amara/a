namespace Bank.api.Models.Responses
{
    public class BankList
    {
        public IList<BankModel> Items { get; set; } = new List<BankModel>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
