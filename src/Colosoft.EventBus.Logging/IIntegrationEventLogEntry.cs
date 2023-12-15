namespace Colosoft.EventBus.Logging
{
    public interface IIntegrationEventLogEntry
    {
        Guid EventId { get; }

        string EventTypeName { get; }

        IntegrationEvent IntegrationEvent { get; }

        EventState State { get; }

        int TimesSent { get; }

        DateTime CreationTime { get; }

        string Content { get; }

        string TransactionId { get; }
    }
}
