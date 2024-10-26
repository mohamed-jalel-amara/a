using System.ComponentModel.DataAnnotations;

namespace Branch.API.Models.Requests
{
    public class BranchRequest
    {
        public string Name { get; set; }
        public string AccessibleFeatures { get; set; }
        public AddressModel Adresse { get; set; }
        public string Bank_id { get; set; }
        public RoutingModel Branch_routing { get; set; }
        public string Branch_type { get; set; }
        public TimeOnly Closing_time { get; set; }
        public TimeOnly Opening_time { get; set; }
        public bool Is_accessible { get; set; }
        public bool Lobby { get; set; }
        public bool Drive_up { get; set; }
        public LocationModel Location { get; set; }
        public MetaModel Meta { get; set; }
        public string More_info { get; set; }
        [RegularExpression("\\+?[1-9][0-9]{10,16}", ErrorMessage = "Phone number is not correct!")]
        public string Phone_number { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}
