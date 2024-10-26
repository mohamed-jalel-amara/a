using System.ComponentModel.DataAnnotations;

namespace Notification.API.Models
{
    public class IdentityMessage
    {
        [RegularExpression("\\+?[1-9][0-9]{10,16}", ErrorMessage = "Phone number is not correct!")]
        public string Destination { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string ToEmail { get; set; }
    }
}
