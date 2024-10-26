using System.Text.Json.Serialization;

namespace Transaction.API.Models
{
    public class TransactionValue
    {
        public string Currency { get; set; }
        public double Amount { get; set; }
        [JsonIgnore]
        public string Account_id { get; set; }
    }
}
