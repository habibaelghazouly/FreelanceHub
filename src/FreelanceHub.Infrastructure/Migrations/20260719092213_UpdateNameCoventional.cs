using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNameCoventional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applications_freelancer_profiles_freelancer_profile_id",
                table: "applications");

            migrationBuilder.RenameColumn(
                name: "freelancer_profile_id",
                table: "applications",
                newName: "FreelancerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_applications_freelancer_profile_id",
                table: "applications",
                newName: "IX_applications_FreelancerProfileId");

            migrationBuilder.AlterColumn<int>(
                name: "FreelancerProfileId",
                table: "applications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "freelancer_user_id",
                table: "applications",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [application]
                SET [freelancer_user_id] = [profile].[user_id]
                FROM [applications] AS [application]
                INNER JOIN [freelancer_profiles] AS [profile]
                    ON [profile].[freelancer_profile_id] = [application].[FreelancerProfileId];

                IF EXISTS (SELECT 1 FROM [applications] WHERE [freelancer_user_id] IS NULL)
                    THROW 51000, 'Unable to map every application to a freelancer user.', 1;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "freelancer_user_id",
                table: "applications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_applications_freelancer_user_id",
                table: "applications",
                column: "freelancer_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_applications_freelancer_profiles_FreelancerProfileId",
                table: "applications",
                column: "FreelancerProfileId",
                principalTable: "freelancer_profiles",
                principalColumn: "freelancer_profile_id");

            migrationBuilder.AddForeignKey(
                name: "FK_applications_users_freelancer_user_id",
                table: "applications",
                column: "freelancer_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applications_freelancer_profiles_FreelancerProfileId",
                table: "applications");

            migrationBuilder.DropForeignKey(
                name: "FK_applications_users_freelancer_user_id",
                table: "applications");

            migrationBuilder.DropIndex(
                name: "IX_applications_freelancer_user_id",
                table: "applications");

            migrationBuilder.Sql(
                """
                UPDATE [application]
                SET [FreelancerProfileId] = [profile].[freelancer_profile_id]
                FROM [applications] AS [application]
                INNER JOIN [freelancer_profiles] AS [profile]
                    ON [profile].[user_id] = [application].[freelancer_user_id];

                IF EXISTS (SELECT 1 FROM [applications] WHERE [FreelancerProfileId] IS NULL)
                    THROW 51000, 'Unable to map every application back to a freelancer profile.', 1;
                """);

            migrationBuilder.DropColumn(
                name: "freelancer_user_id",
                table: "applications");

            migrationBuilder.RenameColumn(
                name: "FreelancerProfileId",
                table: "applications",
                newName: "freelancer_profile_id");

            migrationBuilder.RenameIndex(
                name: "IX_applications_FreelancerProfileId",
                table: "applications",
                newName: "IX_applications_freelancer_profile_id");

            migrationBuilder.AlterColumn<int>(
                name: "freelancer_profile_id",
                table: "applications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_applications_freelancer_profiles_freelancer_profile_id",
                table: "applications",
                column: "freelancer_profile_id",
                principalTable: "freelancer_profiles",
                principalColumn: "freelancer_profile_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
