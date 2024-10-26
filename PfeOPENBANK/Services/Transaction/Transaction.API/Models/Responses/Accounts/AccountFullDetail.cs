using Newtonsoft.Json;

namespace Transaction.API.Models.Responses.Accounts
{
    public class AccountFullDetail
    {
        public long Number { get; set; }
        public string Kind { get; set; }
        [JsonProperty(PropertyName = "IBAN")]
        public string Iban { get; set; }
        public string Swift_bic { get; set; }
        public BanKDetail Bank { get; set; }
    }
}
