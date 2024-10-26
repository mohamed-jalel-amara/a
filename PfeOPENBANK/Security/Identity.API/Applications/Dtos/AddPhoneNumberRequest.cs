using System.ComponentModel.DataAnnotations;

namespace Identity.API.Applications.Dtos
{
    public class AddPhoneNumberRequest
    {
        [RegularExpression("\\+?[1-9][0-9]{10,16}", ErrorMessage = "Phone number is not correct!")]
        public string Number { get; set; }
    }
}
