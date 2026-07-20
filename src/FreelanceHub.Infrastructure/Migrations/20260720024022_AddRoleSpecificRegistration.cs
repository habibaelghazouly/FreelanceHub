using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleSpecificRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [client_profiles])
                    THROW 51000, 'Client profiles must be recreated because client type cannot be inferred safely.', 1;

                IF EXISTS
                (
                    SELECT 1
                    FROM [freelancer_profiles]
                    WHERE NULLIF(LTRIM(RTRIM([professional_title])), '') IS NULL
                        OR DATALENGTH([professional_title]) / 2 > 150
                        OR [hourly_rate] IS NULL
                        OR [hourly_rate] <= 0
                        OR [bio] IS NULL
                        OR LEN(LTRIM(RTRIM([bio]))) < 20
                        OR DATALENGTH([bio]) / 2 > 2000
                        OR [experience_level] NOT IN (30, 31, 32)
                        OR [experience_level] IS NULL
                        OR [availability_status] NOT IN (20, 21, 22)
                        OR [availability_status] IS NULL
                )
                    THROW 51000, 'Freelancer profiles contain incomplete registration details and must be corrected or recreated.', 1;
                """);

            migrationBuilder.DropCheckConstraint(
                name: "chk_freelancer_profiles_availability_status",
                table: "freelancer_profiles");

            migrationBuilder.DropCheckConstraint(
                name: "chk_freelancer_profiles_experience_level",
                table: "freelancer_profiles");

            migrationBuilder.AlterColumn<string>(
                name: "professional_title",
                table: "freelancer_profiles",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "hourly_rate",
                table: "freelancer_profiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "experience_level",
                table: "freelancer_profiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "bio",
                table: "freelancer_profiles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "availability_status",
                table: "freelancer_profiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "company_description",
                table: "client_profiles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "client_type",
                table: "client_profiles",
                type: "int",
                nullable: false);

            migrationBuilder.AddCheckConstraint(
                name: "chk_freelancer_profiles_availability_status",
                table: "freelancer_profiles",
                sql: "[availability_status] IN (20, 21, 22)");

            migrationBuilder.AddCheckConstraint(
                name: "chk_freelancer_profiles_experience_level",
                table: "freelancer_profiles",
                sql: "[experience_level] IN (30, 31, 32)");

            migrationBuilder.AddCheckConstraint(
                name: "chk_freelancer_profiles_hourly_rate",
                table: "freelancer_profiles",
                sql: "[hourly_rate] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "chk_freelancer_profiles_required_details",
                table: "freelancer_profiles",
                sql: "NULLIF(LTRIM(RTRIM([professional_title])), '') IS NOT NULL AND LEN(LTRIM(RTRIM([bio]))) >= 20");

            migrationBuilder.AddCheckConstraint(
                name: "chk_client_profiles_company_details",
                table: "client_profiles",
                sql: "[client_type] = 70 OR ([client_type] = 71 AND NULLIF(LTRIM(RTRIM([company_name])), '') IS NOT NULL AND NULLIF(LTRIM(RTRIM([company_description])), '') IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "chk_client_profiles_type",
                table: "client_profiles",
                sql: "[client_type] IN (70, 71)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_freelancer_profiles_availability_status",
                table: "freelancer_profiles");

            migrationBuilder.DropCheckConstraint(
                name: "chk_freelancer_profiles_experience_level",
                table: "freelancer_profiles");

            migrationBuilder.DropCheckConstraint(
                name: "chk_freelancer_profiles_hourly_rate",
                table: "freelancer_profiles");

            migrationBuilder.DropCheckConstraint(
                name: "chk_freelancer_profiles_required_details",
                table: "freelancer_profiles");

            migrationBuilder.DropCheckConstraint(
                name: "chk_client_profiles_company_details",
                table: "client_profiles");

            migrationBuilder.DropCheckConstraint(
                name: "chk_client_profiles_type",
                table: "client_profiles");

            migrationBuilder.DropColumn(
                name: "client_type",
                table: "client_profiles");

            migrationBuilder.AlterColumn<string>(
                name: "professional_title",
                table: "freelancer_profiles",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<decimal>(
                name: "hourly_rate",
                table: "freelancer_profiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "experience_level",
                table: "freelancer_profiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "bio",
                table: "freelancer_profiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<int>(
                name: "availability_status",
                table: "freelancer_profiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "company_description",
                table: "client_profiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "chk_freelancer_profiles_availability_status",
                table: "freelancer_profiles",
                sql: "[availability_status] IS NULL OR [availability_status] IN (20, 21, 22)");

            migrationBuilder.AddCheckConstraint(
                name: "chk_freelancer_profiles_experience_level",
                table: "freelancer_profiles",
                sql: "[experience_level] IS NULL OR [experience_level] IN (30, 31, 32)");
        }
    }
}
