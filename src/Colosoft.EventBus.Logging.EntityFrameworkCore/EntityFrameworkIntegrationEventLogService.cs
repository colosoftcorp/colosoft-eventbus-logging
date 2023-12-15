using Colosoft.EventBus.Logging.EntityFrameworkCore.Data;
using Colosoft.EventBus.Logging.EntityFrameworkCore.Data.Models;
using System.Text.Json;

namespace Colosoft.EventBus.Logging.EntityFrameworkCore
{
    public class EntityFrameworkIntegrationEventLogService : IIntegrationEventLogService
    {
        private static readonly JsonSerializerOptions IndentedOptions = new () { WriteIndented = true };
        private static readonly JsonSerializerOptions CaseInsensitiveOptions = new () { PropertyNameCaseInsensitive = true };
        private readonly IntegrationEventLogEntryRepository repository;

        public EntityFrameworkIntegrationEventLogService(
            IUnitOfWorkProvider unitOfWorkProvider)
        {
            if (unitOfWorkProvider is null)
            {
                throw new ArgumentNullException(nameof(unitOfWorkProvider));
            }

            this.repository = new IntegrationEventLogEntryRepository(unitOfWorkProvider);
        }

        protected virtual Type ResolveIntegrationEventType(string assemblyName, string typeName)
        {
            return AppDomain.CurrentDomain.Load(assemblyName)?.GetType(typeName, true);
        }

        public async Task<IEnumerable<IIntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId, CancellationToken cancellationToken)
        {
            var entries = (await this.repository.GetByTransactionId(transactionId, cancellationToken)).OrderBy(f => f.CreationTime).ToList();

            foreach (var entry in entries)
            {
                var type = this.ResolveIntegrationEventType(entry.EventAssemblyName, entry.EventTypeName);

                if (type == null)
                {
                    throw new InvalidOperationException($"Type {type} not found.");
                }

                entry.IntegrationEvent = JsonSerializer.Deserialize(entry.Content, type, CaseInsensitiveOptions) as IntegrationEvent;
                entry.Content = null;
            }

            return entries;
        }

        public Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken)
        {
            return this.UpdateEventStatus(eventId, EventState.Published, cancellationToken);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken)
        {
            return this.UpdateEventStatus(eventId, EventState.InProgress, cancellationToken);
        }

        public Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken)
        {
            return this.UpdateEventStatus(eventId, EventState.PublishedFailed, cancellationToken);
        }

        public async Task SaveEventAsync(IntegrationEvent @event, Guid transactionId, CancellationToken cancellationToken)
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var eventType = @event.GetType();

            var logEntry = new IntegrationEventLogEntry();
            logEntry.EventId = @event.Id;
            logEntry.CreationTime = @event.CreationDate;
            logEntry.EventTypeName = eventType.FullName;
            logEntry.EventAssemblyName = eventType.Assembly.FullName;
            logEntry.Content = JsonSerializer.Serialize(@event, @event.GetType(), IndentedOptions);
            logEntry.State = EventState.NotPublished;
            logEntry.TimesSent = 0;
            logEntry.TransactionId = transactionId.ToString();

            await this.repository.Save(logEntry, cancellationToken);
        }

        private async Task UpdateEventStatus(Guid eventId, EventState status, CancellationToken cancellationToken)
        {
            var eventLogEntry = await this.repository.Get(eventId, cancellationToken);
            eventLogEntry.State = status;

            if (status == EventState.InProgress)
            {
                eventLogEntry.TimesSent++;
            }

            await this.repository.Save(eventLogEntry, cancellationToken);
        }
    }
}
