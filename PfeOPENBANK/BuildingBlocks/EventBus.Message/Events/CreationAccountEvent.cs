

namespace EventBus.Message.Events
{
    public class CreationAccountEvent : IntegrationBaseEvent
    {
        public string AccountId { get; set; }
        public string BankId { get; set; }
        public string OwnerId { get; set; }

    }
}
