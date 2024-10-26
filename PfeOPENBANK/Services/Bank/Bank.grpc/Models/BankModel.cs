using Newtonsoft.Json;

namespace Bank.grpc.Models
{
    public class BankModel
    {
        [JsonProperty(PropertyName = "ID")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "SHORT_NAME")]
        public string ShortName { get; set; }
        [JsonProperty(PropertyName = "FULL_NAME")]
        public string FullName { get; set; }
        [JsonProperty(PropertyName = "LOGO")]
        public string Logo { get; set; }
        [JsonProperty(PropertyName = "WEBSITE")]
        public string Website { get; set; }
        [JsonProperty(PropertyName = "ROUTING_SCHEME")]
        public string scheme { get; set; }
        [JsonProperty(PropertyName = "ROUTING_ADDRESS")]
        public string address { get; set; }
        [JsonProperty(PropertyName = "CREATED")]
        public DateTime Created { get; set; }
        [JsonProperty(PropertyName = "UPDATED")]
        public DateTime Updated { get; set; }
        [JsonIgnore]
        public int Code { get; set; } = 0;
    }
}
