namespace EventBus.Message.Events
{
    public class IntegrationBaseEvent
    {
        public IntegrationBaseEvent()
        {
            Id = Guid.NewGuid();
            CreatedTime = DateTime.UtcNow;
        }
        public IntegrationBaseEvent(Guid guid, DateTime createdTime)
        {
            Id = guid;
            this.CreatedTime = createdTime;
        }
        public Guid Id { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}
