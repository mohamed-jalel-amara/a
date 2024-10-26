using System.Text.Json.Serialization;

namespace Bank.api.Models.Responses
{
    public class BankResponse
    {
        public string Id { get; set; }
        public string Short_name { get; set; }
        public string Full_name { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }
        public BankRouting Bank_routing { get; set; }
        [JsonIgnore]
        public int Code { get; set; } = 0;
        [JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}
