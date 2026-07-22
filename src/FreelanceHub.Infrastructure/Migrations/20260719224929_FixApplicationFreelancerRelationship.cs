using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixApplicationFreelancerRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @defaultConstraint sysname;
                SELECT @defaultConstraint = [constraint].[name]
                FROM [sys].[default_constraints] AS [constraint]
                INNER JOIN [sys].[columns] AS [column]
                    ON [column].[object_id] = [constraint].[parent_object_id]
                    AND [column].[column_id] = [constraint].[parent_column_id]
                WHERE [constraint].[parent_object_id] = OBJECT_ID(N'[applications]')
                    AND [column].[name] = N'freelancer_user_id';

                IF @defaultConstraint IS NOT NULL
                    EXEC(N'ALTER TABLE [applications] DROP CONSTRAINT [' + @defaultConstraint + N']');
                """);

            migrationBuilder.Sql(
                """
                UPDATE [application]
                SET [freelancer_user_id] = [profile].[user_id]
                FROM [applications] AS [application]
                INNER JOIN [freelancer_profiles] AS [profile]
                    ON [profile].[freelancer_profile_id] = [application].[FreelancerProfileId]
                WHERE [application].[FreelancerProfileId] IS NOT NULL;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_applications_freelancer_profiles_FreelancerProfileId",
                table: "applications");

            migrationBuilder.DropIndex(
                name: "IX_applications_FreelancerProfileId",
                table: "applications");

            migrationBuilder.DropColumn(
                name: "FreelancerProfileId",
                table: "applications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FreelancerProfileId",
                table: "applications",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [application]
                SET [FreelancerProfileId] = [profile].[freelancer_profile_id]
                FROM [applications] AS [application]
                INNER JOIN [freelancer_profiles] AS [profile]
                    ON [profile].[user_id] = [application].[freelancer_user_id];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_applications_FreelancerProfileId",
                table: "applications",
                column: "FreelancerProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_applications_freelancer_profiles_FreelancerProfileId",
                table: "applications",
                column: "FreelancerProfileId",
                principalTable: "freelancer_profiles",
                principalColumn: "freelancer_profile_id");
        }
    }
}
