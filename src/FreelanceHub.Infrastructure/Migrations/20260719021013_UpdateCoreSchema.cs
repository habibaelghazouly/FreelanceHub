using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FreelanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCoreSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [users] WHERE [user_status] NOT IN (1, 10, 11, 12, 13))
                    THROW 51000, 'Unsupported users.user_status value found before core schema migration.', 1;

                IF EXISTS (SELECT 1 FROM [freelancer_profiles] WHERE [availability_status] IS NOT NULL AND [availability_status] NOT IN (0, 1, 20, 21, 22))
                    THROW 51000, 'Unsupported freelancer_profiles.availability_status value found before core schema migration.', 1;

                IF EXISTS (SELECT 1 FROM [freelancer_profiles] WHERE [experience_level] IS NOT NULL AND [experience_level] NOT IN (N'Beginner', N'Intermediate', N'Expert', N'30', N'31', N'32'))
                    THROW 51000, 'Unsupported freelancer_profiles.experience_level value found before core schema migration.', 1;

                IF EXISTS (SELECT [name] FROM [categories] GROUP BY [name] HAVING COUNT(*) > 1)
                    THROW 51000, 'Duplicate category names must be resolved before core schema migration.', 1;

                IF EXISTS (SELECT [name] FROM [tags] GROUP BY [name] HAVING COUNT(*) > 1)
                    THROW 51000, 'Duplicate tag names must be resolved before core schema migration.', 1;

                IF EXISTS (SELECT 1 FROM [attachments] WHERE [file_size] < 0)
                    THROW 51000, 'Negative attachment file sizes must be resolved before core schema migration.', 1;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "user_status",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 10,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql("UPDATE [users] SET [user_status] = 10 WHERE [user_status] = 1;");
            migrationBuilder.Sql("UPDATE [freelancer_profiles] SET [availability_status] = CASE [availability_status] WHEN 1 THEN 20 WHEN 0 THEN 22 ELSE [availability_status] END WHERE [availability_status] IN (0, 1);");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "assigned_at",
                table: "user_roles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "freelancer_profiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSDATETIME()");

            migrationBuilder.AddColumn<int>(
                name: "experience_level_grouped",
                table: "freelancer_profiles",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [freelancer_profiles]
                SET [experience_level_grouped] = CASE [experience_level]
                    WHEN N'Beginner' THEN 30
                    WHEN N'Intermediate' THEN 31
                    WHEN N'Expert' THEN 32
                    WHEN N'30' THEN 30
                    WHEN N'31' THEN 31
                    WHEN N'32' THEN 32
                    ELSE NULL
                END;
                """);

            migrationBuilder.DropColumn(
                name: "experience_level",
                table: "freelancer_profiles");

            migrationBuilder.RenameColumn(
                name: "experience_level_grouped",
                table: "freelancer_profiles",
                newName: "experience_level");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freelancer_profiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "client_profiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "client_profiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "uploaded_at",
                table: "attachments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSDATETIME()");

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
                    table.ForeignKey(
                        name: "FK_client_profile_attachments_client_profiles_client_profile_id",
                        column: x => x.client_profile_id,
                        principalTable: "client_profiles",
                        principalColumn: "client_profile_id",
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
                    table.ForeignKey(
                        name: "FK_freelancer_profile_attachments_freelancer_profiles_freelancer_profile_id",
                        column: x => x.freelancer_profile_id,
                        principalTable: "freelancer_profiles",
                        principalColumn: "freelancer_profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    client_user_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    budget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    job_status = table.Column<int>(type: "int", nullable: false, defaultValue: 40),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.job_id);
                    table.CheckConstraint("chk_jobs_budget", "[budget] >= 0");
                    table.CheckConstraint("chk_jobs_deleted_at", "([is_deleted] = 0 AND [deleted_at] IS NULL) OR ([is_deleted] = 1 AND [deleted_at] IS NOT NULL)");
                    table.CheckConstraint("chk_jobs_status", "[job_status] IN (40, 41, 42, 43)");
                    table.ForeignKey(
                        name: "FK_jobs_users_client_user_id",
                        column: x => x.client_user_id,
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
                name: "applications",
                columns: table => new
                {
                    application_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    job_id = table.Column<int>(type: "int", nullable: false),
                    freelancer_profile_id = table.Column<int>(type: "int", nullable: false),
                    proposed_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    cover_letter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timeline_days = table.Column<int>(type: "int", nullable: false),
                    application_status = table.Column<int>(type: "int", nullable: false, defaultValue: 50),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.application_id);
                    table.UniqueConstraint("AK_applications_application_id_job_id", x => new { x.application_id, x.job_id });
                    table.CheckConstraint("chk_applications_status", "[application_status] IN (50, 51, 52, 53, 54)");
                    table.ForeignKey(
                        name: "FK_applications_freelancer_profiles_freelancer_profile_id",
                        column: x => x.freelancer_profile_id,
                        principalTable: "freelancer_profiles",
                        principalColumn: "freelancer_profile_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_applications_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Restrict);
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
                    table.ForeignKey(
                        name: "FK_job_attachments_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_categories",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_categories", x => new { x.job_id, x.category_id });
                    table.ForeignKey(
                        name: "FK_job_categories_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "FK_job_categories_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id");
                });

            migrationBuilder.CreateTable(
                name: "job_tags",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "int", nullable: false),
                    tag_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_tags", x => new { x.job_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_job_tags_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id");
                    table.ForeignKey(
                        name: "FK_job_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "tag_id");
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
                        name: "FK_freelancer_skills_freelancer_profiles_freelancer_profile_id",
                        column: x => x.freelancer_profile_id,
                        principalTable: "freelancer_profiles",
                        principalColumn: "freelancer_profile_id");
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
                        name: "FK_job_skills_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id");
                    table.ForeignKey(
                        name: "FK_job_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "skill_id");
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
                        name: "FK_application_attachments_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "application_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_application_attachments_attachments_attachment_id",
                        column: x => x.attachment_id,
                        principalTable: "attachments",
                        principalColumn: "attachment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                columns: table => new
                {
                    contract_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    job_id = table.Column<int>(type: "int", nullable: false),
                    accepted_application_id = table.Column<int>(type: "int", nullable: false),
                    agreed_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    contract_status = table.Column<int>(type: "int", nullable: false, defaultValue: 60),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    expected_completion_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    actual_completion_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts", x => x.contract_id);
                    table.CheckConstraint("chk_contracts_actual_completion", "[actual_completion_date] IS NULL OR [actual_completion_date] >= [start_date]");
                    table.CheckConstraint("chk_contracts_agreed_amount", "[agreed_amount] >= 0");
                    table.CheckConstraint("chk_contracts_expected_completion", "[expected_completion_date] IS NULL OR [expected_completion_date] >= [start_date]");
                    table.CheckConstraint("chk_contracts_status", "[contract_status] IN (60, 61, 62, 63, 64)");
                    table.ForeignKey(
                        name: "FK_contracts_applications_accepted_application_id_job_id",
                        columns: x => new { x.accepted_application_id, x.job_id },
                        principalTable: "applications",
                        principalColumns: new[] { "application_id", "job_id" });
                    table.ForeignKey(
                        name: "FK_contracts_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id");
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
                    table.ForeignKey(
                        name: "FK_contract_attachments_contracts_contract_id",
                        column: x => x.contract_id,
                        principalTable: "contracts",
                        principalColumn: "contract_id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.AddCheckConstraint(
                name: "chk_users_status",
                table: "users",
                sql: "[user_status] IN (10, 11, 12, 13)");

            migrationBuilder.CreateIndex(
                name: "IX_tags_name",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "chk_freelancer_profiles_availability_status",
                table: "freelancer_profiles",
                sql: "[availability_status] IS NULL OR [availability_status] IN (20, 21, 22)");

            migrationBuilder.AddCheckConstraint(
                name: "chk_freelancer_profiles_experience_level",
                table: "freelancer_profiles",
                sql: "[experience_level] IS NULL OR [experience_level] IN (30, 31, 32)");

            migrationBuilder.CreateIndex(
                name: "IX_categories_name",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "chk_attachments_file_size",
                table: "attachments",
                sql: "[file_size] IS NULL OR [file_size] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_application_attachments_attachment_id",
                table: "application_attachments",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_applications_freelancer_profile_id",
                table: "applications",
                column: "freelancer_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_applications_job_id",
                table: "applications",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_client_profile_attachments_attachment_id",
                table: "client_profile_attachments",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_contract_attachments_attachment_id",
                table: "contract_attachments",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_accepted_application_id_job_id",
                table: "contracts",
                columns: new[] { "accepted_application_id", "job_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contracts_job_id",
                table: "contracts",
                column: "job_id",
                unique: true);

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
                name: "IX_job_categories_category_id",
                table: "job_categories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_skills_skill_id",
                table: "job_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_tags_tag_id",
                table: "job_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_client_user_id",
                table: "jobs",
                column: "client_user_id");

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
                name: "job_categories");

            migrationBuilder.DropTable(
                name: "job_skills");

            migrationBuilder.DropTable(
                name: "job_tags");

            migrationBuilder.DropTable(
                name: "contracts");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "applications");

            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropCheckConstraint(
                name: "chk_users_status",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_tags_name",
                table: "tags");

            migrationBuilder.DropCheckConstraint(
                name: "chk_freelancer_profiles_availability_status",
                table: "freelancer_profiles");

            migrationBuilder.DropCheckConstraint(
                name: "chk_freelancer_profiles_experience_level",
                table: "freelancer_profiles");

            migrationBuilder.DropIndex(
                name: "IX_categories_name",
                table: "categories");

            migrationBuilder.DropCheckConstraint(
                name: "chk_attachments_file_size",
                table: "attachments");

            migrationBuilder.Sql("UPDATE [users] SET [user_status] = 1 WHERE [user_status] = 10;");
            migrationBuilder.Sql("UPDATE [freelancer_profiles] SET [availability_status] = CASE [availability_status] WHEN 20 THEN 1 WHEN 21 THEN 0 WHEN 22 THEN 0 ELSE [availability_status] END WHERE [availability_status] IN (20, 21, 22);");

            migrationBuilder.AddColumn<string>(
                name: "experience_level_legacy",
                table: "freelancer_profiles",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [freelancer_profiles]
                SET [experience_level_legacy] = CASE [experience_level]
                    WHEN 30 THEN N'Beginner'
                    WHEN 31 THEN N'Intermediate'
                    WHEN 32 THEN N'Expert'
                    ELSE NULL
                END;
                """);

            migrationBuilder.DropColumn(
                name: "experience_level",
                table: "freelancer_profiles");

            migrationBuilder.RenameColumn(
                name: "experience_level_legacy",
                table: "freelancer_profiles",
                newName: "experience_level");

            migrationBuilder.AlterColumn<int>(
                name: "user_status",
                table: "users",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "assigned_at",
                table: "user_roles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "freelancer_profiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "freelancer_profiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "client_profiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "client_profiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "uploaded_at",
                table: "attachments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");
        }
    }
}
