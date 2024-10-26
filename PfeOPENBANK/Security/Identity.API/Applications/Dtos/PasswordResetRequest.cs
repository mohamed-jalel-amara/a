namespace Identity.API.Applications.Dtos
{
    public class PasswordResetRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
