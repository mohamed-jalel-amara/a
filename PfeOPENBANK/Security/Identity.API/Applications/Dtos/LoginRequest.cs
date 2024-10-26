namespace Identity.API.Applications.Dtos
{
    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
