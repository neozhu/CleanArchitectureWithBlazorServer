using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreatedByUser_ApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_created_by",
                table: "AspNetUsers",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_last_modified_by",
                table: "AspNetUsers",
                column: "last_modified_by");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_asp_net_users_created_by",
                table: "AspNetUsers",
                column: "created_by",
                principalTable: "AspNetUsers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_asp_net_users_last_modified_by",
                table: "AspNetUsers",
                column: "last_modified_by",
                principalTable: "AspNetUsers",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_asp_net_users_created_by",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_asp_net_users_last_modified_by",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_created_by",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_last_modified_by",
                table: "AspNetUsers");
        }
    }
}
