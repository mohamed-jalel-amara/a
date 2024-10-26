using System.Text.Json.Serialization;

namespace Account.API.Models.Responses
{
    public class AccountCreated
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Bank_id { get; set; }
        public string _links { get; set; }
        [JsonIgnore]
        public int Code { get; set; }
        [JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}
