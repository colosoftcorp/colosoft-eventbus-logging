using Colosoft.Data.EntityFrameworkCore;
using Colosoft.EventBus.Logging.EntityFrameworkCore.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Colosoft.EventBus.Logging.EntityFrameworkCore.Data.Mappings
{
    internal class IntegrationEventLogEntryMap : TypeConfiguration<IntegrationEventLogEntry>
    {
        public override void Configure(EntityTypeBuilder<IntegrationEventLogEntry> builder)
        {
            builder.ToTable("IntegrationEventLog");

            builder.HasKey(e => e.EventId);

            builder.Property(e => e.EventId)
                .IsRequired();

            builder.Property(e => e.Content)
                .IsRequired();

            builder.Property(e => e.CreationTime)
                .IsRequired();

            builder.Property(e => e.State)
                .IsRequired();

            builder.Property(e => e.TimesSent)
                .IsRequired();

            builder.Property(e => e.EventTypeName)
                .IsRequired();

            builder.Property(e => e.EventAssemblyName)
               .IsRequired();

            builder.Property(f => f.TransactionId)
                .IsRequired();

            builder.Ignore(f => f.IntegrationEvent);
        }
    }
}
