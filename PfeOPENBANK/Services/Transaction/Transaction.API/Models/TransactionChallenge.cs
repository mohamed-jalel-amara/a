using Newtonsoft.Json;

namespace Transaction.API.Models
{
    public class TransactionChallenge
    {
        [JsonProperty(PropertyName = "ID")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "ALLOWED_ATTEMPS")]
        public int Allowed_attemps { get; set; }
        [JsonProperty(PropertyName = "CHALLENGE_TYPE")]
        public string Challenge_type { get; set; }
        [JsonProperty(PropertyName = "TRANSACTION_IDS")]
        public string Transaction_ids { get; set; }
    }
}
