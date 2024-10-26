using Newtonsoft.Json;

namespace Bank.api.Models
{
    public class BankModel
    {
        [JsonProperty(PropertyName = "ID")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "SHORT_NAME")]
        public string Short_name { get; set; }
        [JsonProperty(PropertyName = "FULL_NAME")]
        public string Full_name { get; set; }
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
    }
}
