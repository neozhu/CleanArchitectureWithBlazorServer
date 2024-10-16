using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationRole_Name_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_documents_users_created_by",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "fk_documents_users_last_modified_by",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_roles_tenant_id",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_roles_tenant_id_name",
                table: "AspNetRoles",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalized_name");

            migrationBuilder.AddForeignKey(
                name: "fk_documents_asp_net_users_created_by",
                table: "documents",
                column: "created_by",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_documents_asp_net_users_last_modified_by",
                table: "documents",
                column: "last_modified_by",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_documents_asp_net_users_created_by",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "fk_documents_asp_net_users_last_modified_by",
                table: "documents");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_roles_tenant_id_name",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_roles_tenant_id",
                table: "AspNetRoles",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_documents_users_created_by",
                table: "documents",
                column: "created_by",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_documents_users_last_modified_by",
                table: "documents",
                column: "last_modified_by",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
