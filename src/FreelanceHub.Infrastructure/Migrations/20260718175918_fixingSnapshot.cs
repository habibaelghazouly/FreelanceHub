using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FreelanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixingSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    normalized_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
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
                name: "tags",
                columns: table => new
                {
                    tag_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.tag_id);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_claims_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "role_id",
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
                });

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
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    user_status = table.Column<int>(type: "int", nullable: false),
                    profile_image_attachment_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    normalized_username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    normalized_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    is_email_verified = table.Column<bool>(type: "bit", nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_attachments_profile_image_attachment_id",
                        column: x => x.profile_image_attachment_id,
                        principalTable: "attachments",
                        principalColumn: "attachment_id");
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
                    job_status = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.job_id);
                    table.ForeignKey(
                        name: "FK_jobs_users_client_user_id",
                        column: x => x.client_user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_claims_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_logins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_user_logins_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_user_tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "user_id",
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
                    contract_status = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    expected_completion_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    actual_completion_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts", x => x.contract_id);
                    table.ForeignKey(
                        name: "FK_contracts_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id");
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "category_id", "name" },
                values: new object[,]
                {
                    { 1, "Web Development" },
                    { 2, "Mobile Development" },
                    { 3, "UI / UX Design" },
                    { 4, "Writing" },
                    { 5, "Marketing" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "role_id", "ConcurrencyStamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { 1, null, "Admin", "ADMIN" },
                    { 2, null, "Client", "CLIENT" },
                    { 3, null, "Freelancer", "FREELANCER" }
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

            migrationBuilder.InsertData(
                table: "tags",
                columns: new[] { "tag_id", "name" },
                values: new object[,]
                {
                    { 1, "C#" },
                    { 2, ".NET" },
                    { 3, "React" },
                    { 4, "SQL" },
                    { 5, "Figma" },
                    { 6, "SEO" },
                    { 7, "API" },
                    { 8, "Content Writing" }
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
                name: "IX_client_profiles_company_logo_attachment_id",
                table: "client_profiles",
                column: "company_logo_attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_client_profiles_user_id",
                table: "client_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contract_attachments_attachment_id",
                table: "contract_attachments",
                column: "attachment_id");

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
                name: "IX_freelancer_profiles_user_id",
                table: "freelancer_profiles",
                column: "user_id",
                unique: true);

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
                name: "IX_jobs_client_user_id",
                table: "jobs",
                column: "client_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_claims_RoleId",
                table: "role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "roles",
                column: "normalized_name",
                unique: true,
                filter: "[normalized_name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_skills_name",
                table: "skills",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_claims_UserId",
                table: "user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_logins_UserId",
                table: "user_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "users",
                column: "normalized_email",
                unique: true,
                filter: "[normalized_email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_profile_image_attachment_id",
                table: "users",
                column: "profile_image_attachment_id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "users",
                column: "normalized_username",
                unique: true,
                filter: "[normalized_username] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_application_attachments_attachments_attachment_id",
                table: "application_attachments",
                column: "attachment_id",
                principalTable: "attachments",
                principalColumn: "attachment_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_attachments_users_uploaded_by_user_id",
                table: "attachments",
                column: "uploaded_by_user_id",
                principalTable: "users",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_attachments_profile_image_attachment_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "application_attachments");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "client_profile_attachments");

            migrationBuilder.DropTable(
                name: "client_profiles");

            migrationBuilder.DropTable(
                name: "contract_attachments");

            migrationBuilder.DropTable(
                name: "contracts");

            migrationBuilder.DropTable(
                name: "freelancer_profile_attachments");

            migrationBuilder.DropTable(
                name: "freelancer_profiles");

            migrationBuilder.DropTable(
                name: "freelancer_skills");

            migrationBuilder.DropTable(
                name: "job_attachments");

            migrationBuilder.DropTable(
                name: "job_skills");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
