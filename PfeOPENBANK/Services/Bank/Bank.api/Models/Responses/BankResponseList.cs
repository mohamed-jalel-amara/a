namespace Bank.api.Models.Responses
{
    public class BankResponseList
    {
        public IList<BankResponse> Banks { get; set; } = new List<BankResponse>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
