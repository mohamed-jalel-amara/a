using System.ComponentModel.DataAnnotations;

namespace Identity.API.Applications.Dtos
{
    public class VerifyPhoneNumberRequest
    {
        [RegularExpression("\\+?[1-9][0-9]{10,16}", ErrorMessage = "Phone number is not correct!")]
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
    }
}
