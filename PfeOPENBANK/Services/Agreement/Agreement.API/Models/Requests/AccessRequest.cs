namespace Agreement.API.Models.Requests
{
    public class AccessRequest
    {
        public int AccessId { get; set; }
        public int Duration { get; set; }
        public ACCESS_STATUS Status { get; set; }
    }
}
