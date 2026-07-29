using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContractStatusConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_contracts_status",
                table: "contracts");

            migrationBuilder.AddCheckConstraint(
                name: "chk_contracts_status",
                table: "contracts",
                sql: "[contract_status] IN (60, 61, 62, 63, 64,65)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_contracts_status",
                table: "contracts");

            migrationBuilder.AddCheckConstraint(
                name: "chk_contracts_status",
                table: "contracts",
                sql: "[contract_status] IN (60, 61, 62, 63, 64)");
        }
    }
}
