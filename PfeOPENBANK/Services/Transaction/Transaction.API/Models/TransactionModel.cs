using Newtonsoft.Json;

namespace Transaction.API.Models
{
    public class TransactionModel
    {
        [JsonProperty(PropertyName = "ID")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "POSTED")]
        public DateTime Posted { get; set; }
        [JsonProperty(PropertyName = "COMPLETED")]
        public DateTime Completed { get; set; }
    }
}
