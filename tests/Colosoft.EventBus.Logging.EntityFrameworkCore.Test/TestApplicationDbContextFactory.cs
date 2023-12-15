using Colosoft.Data.EntityFrameworkCore;
using Colosoft.Data.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Colosoft.EventBus.Logging.EntityFrameworkCore.Test
{
    internal class TestApplicationDbContextFactory : SqliteApplicationDbContextFactory
    {
        public TestApplicationDbContextFactory(
            IEnumerable<ITypeConfiguration> typeConfigurations)
            : base(typeConfigurations)
        {
        }

        protected override DbContext Create(DbContextOptions options) =>
            new TestDbContext(options);
    }
}
