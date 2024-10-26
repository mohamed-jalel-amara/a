using System.ComponentModel.DataAnnotations;

namespace Identity.API.Applications.Dtos
{
    public class ForgotPasswordRequest
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
