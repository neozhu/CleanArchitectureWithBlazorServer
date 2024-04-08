using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddedDataProtectionKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_audit_trails_users_owner_id",
                table: "audit_trails");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "loggers",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "level",
                table: "loggers",
                type: "character varying(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "exception",
                table: "loggers",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "data_protection_keys",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    friendly_name = table.Column<string>(type: "text", nullable: true),
                    xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_protection_keys", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_loggers_exception",
                table: "loggers",
                column: "exception");

            migrationBuilder.CreateIndex(
                name: "ix_loggers_level",
                table: "loggers",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "ix_loggers_message",
                table: "loggers",
                column: "message");

            migrationBuilder.CreateIndex(
                name: "ix_loggers_time_stamp",
                table: "loggers",
                column: "time_stamp");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_tenant_id",
                table: "AspNetUsers",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_tenants_tenant_id",
                table: "AspNetUsers",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_audit_trails_asp_net_users_user_id",
                table: "audit_trails",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_tenants_tenant_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "fk_audit_trails_asp_net_users_user_id",
                table: "audit_trails");

            migrationBuilder.DropTable(
                name: "data_protection_keys");

            migrationBuilder.DropIndex(
                name: "ix_loggers_exception",
                table: "loggers");

            migrationBuilder.DropIndex(
                name: "ix_loggers_level",
                table: "loggers");

            migrationBuilder.DropIndex(
                name: "ix_loggers_message",
                table: "loggers");

            migrationBuilder.DropIndex(
                name: "ix_loggers_time_stamp",
                table: "loggers");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_tenant_id",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "loggers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "level",
                table: "loggers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "exception",
                table: "loggers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_audit_trails_users_owner_id",
                table: "audit_trails",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
