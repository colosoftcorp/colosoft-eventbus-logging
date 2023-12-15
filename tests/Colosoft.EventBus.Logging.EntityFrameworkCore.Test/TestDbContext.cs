using Microsoft.EntityFrameworkCore;

namespace Colosoft.EventBus.Logging.EntityFrameworkCore.Test
{
    internal class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}