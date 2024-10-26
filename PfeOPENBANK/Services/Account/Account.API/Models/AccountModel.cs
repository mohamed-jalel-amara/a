using Account.API.Models.Responses;
using Newtonsoft.Json;

namespace Account.API.Models
{
    public class AccountModel
    {
        [JsonProperty(PropertyName = "ID")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "LABEL")]
        public string Label { get; set; }
        [JsonProperty(PropertyName = "ACCNUMBER")]
        public long AccNumber { get; set; }
        [JsonProperty(PropertyName = "TYPE")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "CURRENCY")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "AMOUNT")]
        public double Amount { get; set; }
        [JsonProperty(PropertyName = "IBAN")]
        public string Iban { get; set; }
        [JsonProperty(PropertyName = "SWIFT_BIC")]
        public string Swift_bic { get; set; }
        [JsonProperty(PropertyName = "BANK_ID")]
        public string Bank_id { get; set; }
        [JsonProperty(PropertyName = "ROUTING_SCHEME")]
        public string Routing_scheme { get; set; }
        [JsonProperty(PropertyName = "ROUTING_ADDRESS")]
        public string Routing_address { get; set; }
        [JsonProperty(PropertyName = "CREATED_AT")]
        public DateTime Created_at { get; set; }
        [JsonProperty(PropertyName = "UPDATED_AT")]
        public DateTime Updated_at { get; set; }
        [JsonProperty(PropertyName = "OWNER_ID")]
        public string Owner_id { get; set; }
    }
}
