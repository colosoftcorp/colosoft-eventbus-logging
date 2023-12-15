namespace Colosoft.EventBus.Logging.EntityFrameworkCore.Data.Models
{
    internal class IntegrationEventLogEntry : IIntegrationEventLogEntry
    {
        public Guid EventId { get; set; }

        public string EventTypeName { get; set; }

        public string EventAssemblyName { get; set; }

        public IntegrationEvent IntegrationEvent { get; set; }

        public EventState State { get; set; }

        public int TimesSent { get; set; }

        public DateTime CreationTime { get; set; }

        public string Content { get; set; }

        public string TransactionId { get; set; }
    }
}
