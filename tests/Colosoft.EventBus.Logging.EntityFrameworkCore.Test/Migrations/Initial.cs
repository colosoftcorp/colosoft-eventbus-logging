using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Colosoft.EventBus.Logging.EntityFrameworkCore.Test.Migrations
{
    [DbContext(typeof(TestDbContext))]
    [Migration("202312131420_Initial")]
    internal class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegrationEventLog",
                columns: table => new
                {
                    EventId = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    EventTypeName = table.Column<string>(nullable: false),
                    EventAssemblyName = table.Column<string>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TimesSent = table.Column<int>(nullable: false),
                    TransactionId = table.Column<string>(maxLength: 200, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLog", x => x.EventId);
                });
        }
    }
}
