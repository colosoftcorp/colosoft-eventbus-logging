using Colosoft.Data;

namespace Colosoft.EventBus.Logging.EntityFrameworkCore.Test
{
    internal class DbConnectionConfiguration : IDbConnectionConfiguration
    {
        public string ProviderName => "Default";

        public string ConnectionString => "Data Source=:memory:";
    }
}
