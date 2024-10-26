using System.ComponentModel.DataAnnotations;

namespace Identity.API.Applications.Dtos
{
    public class ConfirmEmailRequest
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
