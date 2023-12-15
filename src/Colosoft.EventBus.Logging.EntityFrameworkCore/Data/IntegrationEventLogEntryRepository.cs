using Colosoft.Data.EntityFrameworkCore;
using Colosoft.EventBus.Logging.EntityFrameworkCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Colosoft.EventBus.Logging.EntityFrameworkCore.Data
{
    internal class IntegrationEventLogEntryRepository : EntityFrameworkRepository<IntegrationEventLogEntry>
    {
        public IntegrationEventLogEntryRepository(IUnitOfWorkProvider unitOfWorkProvider)
            : base(unitOfWorkProvider)
        {
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> GetByTransactionId(Guid transactionId, CancellationToken cancellationToken)
        {
            return await this.DbSet
                .Where(f => f.TransactionId == transactionId.ToString())
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public Task<IntegrationEventLogEntry> Get(Guid eventId, CancellationToken cancellationToken)
        {
            return this.DbSet
                .FirstOrDefaultAsync(f => f.EventId == eventId, cancellationToken);
        }

        public async Task Save(IntegrationEventLogEntry integrationEventLogEntry, CancellationToken cancellationToken)
        {
            var entry = this.Context.Entry(integrationEventLogEntry);

            if (entry != null && entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Modified;
            }
            else
            {
                this.DbSet.Add(integrationEventLogEntry);
            }

            await this.Context.SaveChangesAsync(cancellationToken);
        }
    }
}
