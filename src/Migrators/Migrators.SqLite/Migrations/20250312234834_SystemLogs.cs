using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.SqLite.Migrations
{
    /// <inheritdoc />
    public partial class SystemLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loggers");

            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Message = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    MessageTemplate = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    Level = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Exception = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ClientIP = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ClientAgent = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Properties = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    LogEvent = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_Level",
                table: "SystemLogs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_TimeStamp",
                table: "SystemLogs",
                column: "TimeStamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.CreateTable(
                name: "Loggers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientAgent = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ClientIP = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Exception = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    Level = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    LogEvent = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    Message = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    MessageTemplate = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    Properties = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loggers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loggers_Level",
                table: "Loggers",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Loggers_TimeStamp",
                table: "Loggers",
                column: "TimeStamp");
        }
    }
}
