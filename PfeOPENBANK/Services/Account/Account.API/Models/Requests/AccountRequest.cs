using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Account.API.Models.Requests
{
    public class AccountRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? User_id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Label { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Type { get; set; }
        public BalanceModel Balance { get; set; }
    }
}
