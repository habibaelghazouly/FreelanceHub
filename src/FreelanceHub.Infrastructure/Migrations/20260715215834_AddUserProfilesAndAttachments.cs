using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfilesAndAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "profile_image_attachment_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "attachments",
                columns: table => new
                {
                    attachment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    uploaded_by_user_id = table.Column<int>(type: "int", nullable: false),
                    original_file_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    stored_file_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    file_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    content_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    file_size = table.Column<long>(type: "bigint", nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attachments", x => x.attachment_id);
                    table.ForeignKey(
                        name: "FK_attachments_users_uploaded_by_user_id",
                        column: x => x.uploaded_by_user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "freelancer_profiles",
                columns: table => new
                {
                    freelancer_profile_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    professional_title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    hourly_rate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    experience_level = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    availability_status = table.Column<int>(type: "int", nullable: true),
                    external_portfolio_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    rating_averge = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    rating_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_freelancer_profiles", x => x.freelancer_profile_id);
                    table.ForeignKey(
                        name: "FK_freelancer_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "client_profiles",
                columns: table => new
                {
                    client_profile_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    company_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    company_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company_website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    company_logo_attachment_id = table.Column<int>(type: "int", nullable: true),
                    rating_averge = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    rating_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_profiles", x => x.client_profile_id);
                    table.ForeignKey(
                        name: "FK_client_profiles_attachments_company_logo_attachment_id",
                        column: x => x.company_logo_attachment_id,
                        principalTable: "attachments",
                        principalColumn: "attachment_id");
                    table.ForeignKey(
                        name: "FK_client_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_profile_image_attachment_id",
                table: "users",
                column: "profile_image_attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_attachments_uploaded_by_user_id",
                table: "attachments",
                column: "uploaded_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_client_profiles_company_logo_attachment_id",
                table: "client_profiles",
                column: "company_logo_attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_client_profiles_user_id",
                table: "client_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_freelancer_profiles_user_id",
                table: "freelancer_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_attachments_profile_image_attachment_id",
                table: "users",
                column: "profile_image_attachment_id",
                principalTable: "attachments",
                principalColumn: "attachment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_attachments_profile_image_attachment_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "client_profiles");

            migrationBuilder.DropTable(
                name: "freelancer_profiles");

            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropIndex(
                name: "IX_users_profile_image_attachment_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "profile_image_attachment_id",
                table: "users");
        }
    }
}
