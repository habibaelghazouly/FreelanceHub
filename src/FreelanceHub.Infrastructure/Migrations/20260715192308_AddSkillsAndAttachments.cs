using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FreelanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillsAndAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    table.CheckConstraint("chk_attachments_file_size", "[file_size] IS NULL OR [file_size] >= 0");
                    table.ForeignKey(
                        name: "FK_attachments_users_uploaded_by_user_id",
                        column: x => x.uploaded_by_user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    skill_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.skill_id);
                });

            migrationBuilder.CreateTable(
                name: "application_attachments",
                columns: table => new
                {
                    application_id = table.Column<int>(type: "int", nullable: false),
                    attachment_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_attachments", x => new { x.application_id, x.attachment_id });
                    table.ForeignKey(
                        name: "FK_application_attachments_attachments_attachment_id",
                        column: x => x.attachment_id,
                        principalTable: "attachments",
                        principalColumn: "attachment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "client_profile_attachments",
                columns: table => new
                {
                    client_profile_id = table.Column<int>(type: "int", nullable: false),
                    attachment_id = table.Column<int>(type: "int", nullable: false),
                    attachment_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_profile_attachments", x => new { x.client_profile_id, x.attachment_id });
                    table.ForeignKey(
                        name: "FK_client_profile_attachments_attachments_attachment_id",
                        column: x => x.attachment_id,
                        principalTable: "attachments",
                        principalColumn: "attachment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contract_attachments",
                columns: table => new
                {
                    contract_id = table.Column<int>(type: "int", nullable: false),
                    attachment_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_attachments", x => new { x.contract_id, x.attachment_id });
                    table.ForeignKey(
                        name: "FK_contract_attachments_attachments_attachment_id",
                        column: x => x.attachment_id,
                        principalTable: "attachments",
                        principalColumn: "attachment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "freelancer_profile_attachments",
                columns: table => new
                {
                    freelancer_profile_id = table.Column<int>(type: "int", nullable: false),
                    attachment_id = table.Column<int>(type: "int", nullable: false),
                    attachment_description = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_freelancer_profile_attachments", x => new { x.freelancer_profile_id, x.attachment_id });
                    table.ForeignKey(
                        name: "FK_freelancer_profile_attachments_attachments_attachment_id",
                        column: x => x.attachment_id,
                        principalTable: "attachments",
                        principalColumn: "attachment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_attachments",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "int", nullable: false),
                    attachment_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_attachments", x => new { x.job_id, x.attachment_id });
                    table.ForeignKey(
                        name: "FK_job_attachments_attachments_attachment_id",
                        column: x => x.attachment_id,
                        principalTable: "attachments",
                        principalColumn: "attachment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "freelancer_skills",
                columns: table => new
                {
                    freelancer_profile_id = table.Column<int>(type: "int", nullable: false),
                    skill_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_freelancer_skills", x => new { x.freelancer_profile_id, x.skill_id });
                    table.ForeignKey(
                        name: "FK_freelancer_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id");
                });

            migrationBuilder.CreateTable(
                name: "job_skills",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "int", nullable: false),
                    skill_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_skills", x => new { x.job_id, x.skill_id });
                    table.ForeignKey(
                        name: "FK_job_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id");
                });

            migrationBuilder.InsertData(
                table: "skills",
                columns: new[] { "skill_id", "name" },
                values: new object[,]
                {
                    { 1, "JavaScript" },
                    { 2, "TypeScript" },
                    { 3, "React" },
                    { 4, "Angular" },
                    { 5, "Vue.js" },
                    { 6, "Node.js" },
                    { 7, "ASP.NET Core" },
                    { 8, "PHP" },
                    { 9, "Laravel" },
                    { 10, "Python" },
                    { 11, "Django" },
                    { 12, "Ruby on Rails" },
                    { 13, "WordPress" },
                    { 14, "HTML/CSS" },
                    { 15, "Swift" },
                    { 16, "Kotlin" },
                    { 17, "Flutter" },
                    { 18, "React Native" },
                    { 19, "UI/UX Design" },
                    { 20, "Figma" },
                    { 21, "Adobe Photoshop" },
                    { 22, "Adobe Illustrator" },
                    { 23, "Graphic Design" },
                    { 24, "Logo Design" },
                    { 25, "Content Writing" },
                    { 26, "Copywriting" },
                    { 27, "Technical Writing" },
                    { 28, "Translation" },
                    { 29, "Editing & Proofreading" },
                    { 30, "SEO" },
                    { 31, "Social Media Marketing" },
                    { 32, "Email Marketing" },
                    { 33, "Google Ads" },
                    { 34, "Data Analysis" },
                    { 35, "SQL" },
                    { 36, "Machine Learning" },
                    { 37, "Power BI" },
                    { 38, "Docker" },
                    { 39, "Kubernetes" },
                    { 40, "AWS" },
                    { 41, "Azure" },
                    { 42, "CI/CD" },
                    { 43, "Project Management" },
                    { 44, "Virtual Assistance" },
                    { 45, "Bookkeeping" },
                    { 46, "Video Editing" },
                    { 47, "Voice Over" },
                    { 48, "Motion Graphics" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_attachments_attachment_id",
                table: "application_attachments",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_attachments_uploaded_by_user_id",
                table: "attachments",
                column: "uploaded_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_client_profile_attachments_attachment_id",
                table: "client_profile_attachments",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_attachments_attachment_id",
                table: "contract_attachments",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_freelancer_profile_attachments_attachment_id",
                table: "freelancer_profile_attachments",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_freelancer_skills_skill_id",
                table: "freelancer_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_attachments_attachment_id",
                table: "job_attachments",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_skills_skill_id",
                table: "job_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_skills_name",
                table: "skills",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_attachments");

            migrationBuilder.DropTable(
                name: "client_profile_attachments");

            migrationBuilder.DropTable(
                name: "contract_attachments");

            migrationBuilder.DropTable(
                name: "freelancer_profile_attachments");

            migrationBuilder.DropTable(
                name: "freelancer_skills");

            migrationBuilder.DropTable(
                name: "job_attachments");

            migrationBuilder.DropTable(
                name: "job_skills");

            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropTable(
                name: "skills");
        }
    }
}
