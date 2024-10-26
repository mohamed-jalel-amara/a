using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Message.Events
{
    public class GrantEntitlementEvent : IntegrationBaseEvent
    {
        public string UserId { get; set; }
        public string BankId { get; set; }
        public string EntitlementName { get; set; }
    }
}
