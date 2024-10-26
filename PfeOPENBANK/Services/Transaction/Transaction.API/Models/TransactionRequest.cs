using Newtonsoft.Json;

namespace Transaction.API.Models
{
    public class TransactionRequest
    {
        [JsonProperty(PropertyName = "ID")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "TYPE")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "FROM_BANK_ID")]
        public string From_bank_id { get; set; }
        [JsonProperty(PropertyName = "FROM_ACCOUNT_ID")]
        public string From_account_id { get; set; }
        [JsonProperty(PropertyName = "TO_BANK_ID")]
        public string To_bank_id { get; set; }
        [JsonProperty(PropertyName = "TO_ACCOUNT_ID")]
        public string To_account_id { get; set; }
        [JsonProperty(PropertyName = "CURRENCY")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "AMOUNT")]
        public double Amount { get; set; }
        [JsonProperty(PropertyName = "DESCRIPTION")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "TRANSACTION_IDS")]
        public string Transaction_ids { get; set; }
        [JsonProperty(PropertyName = "STATUS")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "START_DATE")]
        public DateTime Start_date { get; set; }
        [JsonProperty(PropertyName = "END_DATE")]
        public DateTime? End_Date { get; set; }
        [JsonProperty(PropertyName = "SUMMARY")]
        public string? Summary { get; set; }
        [JsonProperty(PropertyName = "TRANSFERT_TYPE")]
        public string? Transfert_type { get; set; }
        [JsonProperty(PropertyName = "FUTURE_DATE")]
        public DateTime? Furture_Date { get; set; }
    }
}
