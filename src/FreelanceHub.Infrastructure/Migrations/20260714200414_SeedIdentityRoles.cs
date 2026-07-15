using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [roles] WHERE [normalized_name] = N'ADMIN')
                    OR NOT EXISTS (SELECT 1 FROM [roles] WHERE [normalized_name] = N'CLIENT')
                    OR NOT EXISTS (SELECT 1 FROM [roles] WHERE [normalized_name] = N'FREELANCER')
                BEGIN
                    SET IDENTITY_INSERT [roles] ON;

                    IF NOT EXISTS (SELECT 1 FROM [roles] WHERE [normalized_name] = N'ADMIN')
                    BEGIN
                        INSERT INTO [roles] ([role_id], [name], [normalized_name])
                        VALUES (1, N'Admin', N'ADMIN');
                    END

                    IF NOT EXISTS (SELECT 1 FROM [roles] WHERE [normalized_name] = N'CLIENT')
                    BEGIN
                        INSERT INTO [roles] ([role_id], [name], [normalized_name])
                        VALUES (2, N'Client', N'CLIENT');
                    END

                    IF NOT EXISTS (SELECT 1 FROM [roles] WHERE [normalized_name] = N'FREELANCER')
                    BEGIN
                        INSERT INTO [roles] ([role_id], [name], [normalized_name])
                        VALUES (3, N'Freelancer', N'FREELANCER');
                    END

                    SET IDENTITY_INSERT [roles] OFF;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [roles]
                WHERE ([role_id] = 1 AND [normalized_name] = N'ADMIN')
                    OR ([role_id] = 2 AND [normalized_name] = N'CLIENT')
                    OR ([role_id] = 3 AND [normalized_name] = N'FREELANCER');
                """);
        }
    }
}
