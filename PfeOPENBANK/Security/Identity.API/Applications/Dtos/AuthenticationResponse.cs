using Microsoft.AspNetCore.Authentication;

namespace Identity.API.Applications.Dtos
{
    public class AuthenticationResponse
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
