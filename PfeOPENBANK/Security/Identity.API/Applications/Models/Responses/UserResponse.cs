
namespace Identity.API.Applications.Models.Responses
{
    public class UserResponse
    {
        public string User_id { get; set; }
        public string Email { get; set; }
        public string Provider_id { get; set; }
        public string Provider { get; set; }
        public string Username { get; set; }
        public Entitlements Entitlements { get; set; }
    }
}
