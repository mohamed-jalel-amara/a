using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Message.Events
{
    public class SendSmsEvent : IntegrationBaseEvent
    {
        [RegularExpression("\\+?[1-9][0-9]{10,16}", ErrorMessage = "Phone number is not correct!")]
        public string Destination { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string ToEmail { get; set; }
    }
}
