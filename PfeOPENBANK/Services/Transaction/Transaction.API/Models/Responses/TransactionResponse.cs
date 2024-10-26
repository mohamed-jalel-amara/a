using System.Text.Json.Serialization;
using Transaction.API.Models.Responses.Accounts;

namespace Transaction.API.Models.Responses
{
    public class TransactionResponse
    {
        public string Id { get; set; }
        public FromAccount This_account { get; set; }
        public ToAccount Other_account { get; set; }
        public TransactionDetail Details { get; set; }
        [JsonIgnore]
        public int Code { get; set; } = 0;
        [JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}
