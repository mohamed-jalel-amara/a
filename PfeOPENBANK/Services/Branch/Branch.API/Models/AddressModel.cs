namespace Branch.API.Models
{
    public class AddressModel
    {
        public string Line_1 { get; set; }
        public string Line_2 { get; set; }
        public string Line_3 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public int Postcode { get; set; }
        public string Country_code { get; set; }
    }
}
