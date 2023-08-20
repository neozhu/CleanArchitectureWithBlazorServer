using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class multiTenantWithEachRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "AspNetUserRoles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUserRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "TenantType",
                table: "AspNetRoles",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_TenantId",
                table: "AspNetUserRoles",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Tenants_TenantId",
                table: "AspNetUserRoles",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_Tenants_TenantId",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_TenantId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "TenantType",
                table: "AspNetRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });
        }
    }
}
