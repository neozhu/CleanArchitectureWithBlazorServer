using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.SqLite.Migrations
{
    /// <inheritdoc />
    public partial class LoginAudit_Id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LoginAudits",
                type: "INTEGER",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 36)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "LoginAudits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "LoginAudits",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "LoginAudits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "LoginAudits",
                type: "TEXT",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "LoginAudits");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LoginAudits");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "LoginAudits");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "LoginAudits");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "LoginAudits",
                type: "TEXT",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldMaxLength: 36)
                .OldAnnotation("Sqlite:Autoincrement", true);
        }
    }
}
