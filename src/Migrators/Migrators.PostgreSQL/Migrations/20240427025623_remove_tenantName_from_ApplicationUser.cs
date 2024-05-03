using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class remove_tenantName_from_ApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tenant_name",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tenant_name",
                table: "AspNetUsers",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);
        }
    }
}
