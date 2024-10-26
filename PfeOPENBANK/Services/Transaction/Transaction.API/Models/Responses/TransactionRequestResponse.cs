using Transaction.API.Models.Responses.Accounts;

namespace Transaction.API.Models.Responses
{
    public class TransactionRequestResponse
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public TransactionRequestFromAccount From { get; set; }
        public TransactionRequestDetails Details { get; set; }
        public IList<string> Transaction_ids { get; set; } = new List<string>();
        public string Status { get; set; }
        public DateTime Start_date { get; set; }
        public DateTime? End_date { get; set; }
        public TransactionChallengeResponse? Challenge { get; set; }
        public TransactionCharge? Charge { get; set; }
    }
}
