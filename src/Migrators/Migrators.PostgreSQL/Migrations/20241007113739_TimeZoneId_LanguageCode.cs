using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class TimeZoneId_LanguageCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "profile_picture_data_url",
                table: "AspNetUsers",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "AspNetUsers",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "language_code",
                table: "AspNetUsers",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "AspNetUsers",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "time_zone_id",
                table: "AspNetUsers",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                table: "AspNetRoles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "AspNetRoles",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified",
                table: "AspNetRoles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "AspNetRoles",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "language_code",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "time_zone_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "created",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "AspNetRoles");

            migrationBuilder.AlterColumn<string>(
                name: "profile_picture_data_url",
                table: "AspNetUsers",
                type: "text",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldMaxLength: 450,
                oldNullable: true);
        }
    }
}
