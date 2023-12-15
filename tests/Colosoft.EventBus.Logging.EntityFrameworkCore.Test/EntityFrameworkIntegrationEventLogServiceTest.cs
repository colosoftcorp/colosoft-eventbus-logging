using Colosoft.Data;
using Colosoft.Data.EntityFrameworkCore;
using Colosoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Colosoft.EventBus.Logging.EntityFrameworkCore.Test
{
    public class EntityFrameworkIntegrationEventLogServiceTest
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IServiceProvider serviceProvider;

        public EntityFrameworkIntegrationEventLogServiceTest()
        {
            var dbConfiguration = new DbConnectionConfiguration();
            var dbConfigurationProvider = new DbConnectionConfigurationProvider(dbConfiguration);

            var services = new ServiceCollection();

            services.AddColosoftEFServices(typeof(EntityFrameworkIntegrationEventLogService).Assembly);

            services
                .AddScoped<IIntegrationEventLogService, EntityFrameworkIntegrationEventLogService>()
                .AddScoped<IApplicationDbContextFactory, TestApplicationDbContextFactory>()
                .AddScoped<IUnitOfWorkFactory>(serviceProvider =>
                    new EntityFrameworkDbUnitOfWorkFactory(
                            dbConfigurationProvider,
                            new SqliteDbProviderFactoryProvider(),
                            serviceProvider.GetRequiredService<IApplicationDbContextFactory>()))
                .AddScoped<IUnitOfWorkRegistry, InstanceUnitOfWorkRegistry>()
                .AddScoped<IUnitOfWorkProvider, UnitOfWorkProvider>();

            this.serviceProvider = services.BuildServiceProvider();

            this.unitOfWorkProvider = this.serviceProvider.GetRequiredService<IUnitOfWorkProvider>();
        }

        private IDisposable CreateUnitOfWork()
        {
            var uow = (IEntityFrameworkUnitOfWork)this.unitOfWorkProvider.Create();
            uow.Context.Database.Migrate();
            return uow;
        }

        [Fact]
        public async Task SaveAndRecorveryIntegrationEvent()
        {
            using (var uow = this.CreateUnitOfWork())
            {
                var transactionId = Guid.NewGuid();
                var logService = this.serviceProvider.GetRequiredService<IIntegrationEventLogService>();

                var @event = new TestIntegrationEvent
                {
                    Message = "Test",
                };

                await logService.SaveEventAsync(@event, transactionId, default);

                var pending = await logService.RetrieveEventLogsPendingToPublishAsync(transactionId, default);

                Assert.NotEmpty(pending);
                Assert.Equal("Test", ((TestIntegrationEvent)pending.First().IntegrationEvent).Message);

                await logService.MarkEventAsFailedAsync(pending.First().EventId, default);

                pending = await logService.RetrieveEventLogsPendingToPublishAsync(transactionId, default);

                Assert.Equal(EventState.PublishedFailed, pending.First().State);
            }
        }

        private sealed record TestIntegrationEvent : IntegrationEvent
        {
            public string Message { get; set;  }
        }
    }
}