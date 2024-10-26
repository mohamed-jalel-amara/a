using Newtonsoft.Json;

namespace Account.Access.API.Models
{
    public class AccountUserAccess
    {
        [JsonProperty(propertyName: "ID")]
        public int Id { get; set; }
        [JsonProperty(propertyName: "VIEW_ID")]
        public int View_id { get; set; }
        [JsonProperty(propertyName: "PROVIDER")]
        public string Provider { get; set; }
        [JsonProperty(propertyName: "PROVIDER_ID")]
        public string Provider_Id { get; set; }
    }
}
