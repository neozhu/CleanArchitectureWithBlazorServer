using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class remove_message_index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Loggers_Message",
                table: "Loggers");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Loggers",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1700)",
                oldMaxLength: 1700,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Loggers",
                type: "nvarchar(1700)",
                maxLength: 1700,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 2147483647,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loggers_Message",
                table: "Loggers",
                column: "Message");
        }
    }
}
