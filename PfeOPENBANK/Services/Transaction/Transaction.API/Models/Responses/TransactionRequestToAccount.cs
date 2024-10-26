using Transaction.API.Models.Responses.Accounts;

namespace Transaction.API.Models.Responses
{
    public class TransactionRequestToAccount
    {
        public TransactionValue Value { get; set; }
        public string? Description { get; set; }
        public string? Transfert_type { get; set; }
        public DateTime? Future_date { get; set; }
        public ToAccountDetail To { get; set; }
    }
}
