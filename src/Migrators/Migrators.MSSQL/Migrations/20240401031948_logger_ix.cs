using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class logger_ix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Loggers_Level_Message_Exception",
                table: "Loggers");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Loggers",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loggers_Exception",
                table: "Loggers",
                column: "Exception");

            migrationBuilder.CreateIndex(
                name: "IX_Loggers_Level",
                table: "Loggers",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Loggers_Message",
                table: "Loggers",
                column: "Message");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Loggers_Exception",
                table: "Loggers");

            migrationBuilder.DropIndex(
                name: "IX_Loggers_Level",
                table: "Loggers");

            migrationBuilder.DropIndex(
                name: "IX_Loggers_Message",
                table: "Loggers");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Loggers",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loggers_Level_Message_Exception",
                table: "Loggers",
                columns: new[] { "Level", "Message", "Exception" });
        }
    }
}
