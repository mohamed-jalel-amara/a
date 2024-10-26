using System.ComponentModel.DataAnnotations;

namespace Identity.API.Applications.Dtos
{
    public class RegisterRequest
    {
        
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
