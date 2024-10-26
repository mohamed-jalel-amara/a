using System.ComponentModel.DataAnnotations;

namespace Identity.API.Applications.Dtos
{
    public class AddUserRequest
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
