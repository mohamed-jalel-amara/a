using System.Text.Json.Serialization;

namespace Branch.API.Models.Response
{
    public class BranchResponse
    {
        public string Id { get; set; }
        public string Bank_id { get; set; }
        public string Name { get; set; }
        public AddressModel Address { get; set; }
        public LocationModel Location { get; set; }
        public MetaModel Meta { get; set; }
        public LobbyModel Lobby { get; set; }
        public DriveUpModel Drive_up { get; set; }
        public RoutingModel Branch_routing { get; set; }
        public bool Is_accessible { get; set; }
        public string AccessibleFeatures { get; set; }
        public string Branch_type { get; set; }
        public string More_info { get; set; }
        public string Phone_Number { get; set; }
        [JsonIgnore]
        public int Code { get; set; }
        [JsonIgnore]
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
