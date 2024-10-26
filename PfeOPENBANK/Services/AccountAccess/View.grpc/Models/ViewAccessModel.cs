using Newtonsoft.Json;

namespace View.grpc.Models
{
    public class ViewAccessModel
    {
        [JsonProperty(PropertyName = "ID")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "VIEW_ID")]
        public int ViewId { get; set; }
        [JsonProperty(PropertyName = "PROVIDER")]
        public string Provider { get; set; }
        [JsonProperty(PropertyName = "PROVIDER_ID")]
        public string ProviderId { get; set; }
    }
}
