using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CleanArchitecture.Blazor.Migrators.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    description = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "data_protection_keys",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    friendly_name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    xml = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_protection_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "picklist_sets",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    text = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    last_modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_by_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_picklist_sets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    description = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    brand = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    unit = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    pictures = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    last_modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_by_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "system_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message = table.Column<string>(type: "text", maxLength: 2147483647, nullable: true),
                    message_template = table.Column<string>(type: "text", maxLength: 2147483647, nullable: true),
                    level = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    time_stamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    exception = table.Column<string>(type: "text", maxLength: 2147483647, nullable: true),
                    user_name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    client_ip = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    client_agent = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    properties = table.Column<string>(type: "text", maxLength: 2147483647, nullable: true),
                    log_event = table.Column<string>(type: "text", maxLength: 2147483647, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    description = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    group = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    role_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    claim_type = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    claim_value = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    display_name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    provider = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    tenant_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    profile_picture_data_url = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_live = table.Column<bool>(type: "boolean", nullable: false),
                    refresh_token = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    superior_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    time_zone_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    language_code = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    security_stamp = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_users_asp_net_users_superior_id",
                        column: x => x.superior_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_asp_net_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    claim_type = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    claim_value = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    provider_key = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    provider_display_name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserPasskeys",
                columns: table => new
                {
                    credential_id = table.Column<byte[]>(type: "bytea", maxLength: 1024, nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_passkeys", x => x.credential_id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_passkeys_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    role_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    login_provider = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    value = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_trails",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    audit_type = table.Column<string>(type: "text", nullable: false),
                    table_name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    changes = table.Column<string>(type: "text", nullable: true),
                    affected_columns = table.Column<List<string>>(type: "text[]", nullable: true),
                    primary_key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_trails", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_trails_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    email = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    country = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    last_modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_by_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contacts", x => x.id);
                    table.ForeignKey(
                        name: "fk_contacts_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_contacts_users_last_modified_by_id",
                        column: x => x.last_modified_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    description = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    url = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    document_type = table.Column<string>(type: "text", nullable: false),
                    tenant_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    last_modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modified_by_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_documents_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_documents_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_documents_users_last_modified_by_id",
                        column: x => x.last_modified_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_users",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    tenant_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_tenant_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tenant_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_passkeys_user_id",
                table: "AspNetUserPasskeys",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "AspNetUserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_superior_id",
                table: "AspNetUsers",
                column: "superior_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_tenant_id",
                table: "AspNetUsers",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_audit_trails_user_id",
                table: "audit_trails",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_contacts_created_by_id",
                table: "contacts",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_contacts_last_modified_by_id",
                table: "contacts",
                column: "last_modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_documents_created_by_id",
                table: "documents",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_documents_last_modified_by_id",
                table: "documents",
                column: "last_modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_documents_tenant_id",
                table: "documents",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_picklist_sets_name_value",
                table: "picklist_sets",
                columns: new[] { "name", "value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_name",
                table: "products",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_system_logs_level",
                table: "system_logs",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "ix_system_logs_time_stamp",
                table: "system_logs",
                column: "time_stamp");

            migrationBuilder.CreateIndex(
                name: "ix_tenant_users_tenant_id",
                table: "tenant_users",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_tenant_users_user_id",
                table: "tenant_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_name",
                table: "tenants",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserPasskeys");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "audit_trails");

            migrationBuilder.DropTable(
                name: "contacts");

            migrationBuilder.DropTable(
                name: "data_protection_keys");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "picklist_sets");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "system_logs");

            migrationBuilder.DropTable(
                name: "tenant_users");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "tenants");
        }
    }
}
