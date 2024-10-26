namespace Account.Access.API.Models.Responses
{
    public class AccountAccessResponseList
    {
        public IList<AccountAccessResponse> Views { get; set; } = new List<AccountAccessResponse>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
