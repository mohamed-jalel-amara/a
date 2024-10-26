namespace Statement.API.Models.Responses
{
    public class StatementList
    {
        public IList<StatementModel> Items { get; set; } = new List<StatementModel>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
