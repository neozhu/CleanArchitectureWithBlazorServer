using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.SqLite.Migrations
{
    /// <inheritdoc />
    public partial class UserLoginRiskSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLoginRiskSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", maxLength: 36, nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    RiskLevel = table.Column<string>(type: "TEXT", nullable: false),
                    RiskScore = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Advice = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginRiskSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLoginRiskSummaries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginRiskSummaries_UserId",
                table: "UserLoginRiskSummaries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginRiskSummaries_UserName",
                table: "UserLoginRiskSummaries",
                column: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLoginRiskSummaries");
        }
    }
}
