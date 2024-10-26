namespace Account.API.Models.Requests
{
    public class UpdateLabelRequest
    {
        public string Bank_id { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }
    }
}
