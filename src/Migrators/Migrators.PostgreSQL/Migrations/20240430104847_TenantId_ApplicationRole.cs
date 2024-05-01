using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class TenantId_ApplicationRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "AspNetRoles",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_roles_tenant_id",
                table: "AspNetRoles",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_roles_tenants_tenant_id",
                table: "AspNetRoles",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_roles_tenants_tenant_id",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_roles_tenant_id",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "AspNetRoles");
        }
    }
}
