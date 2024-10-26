
namespace Transaction.API.Models.Responses
{
    public class TransactionChallengeResponse
    {
        public string Id { get; set; }
        public int Allowed_attemps { get; set; }
        public string Challenge_type { get; set; }
    }
}
