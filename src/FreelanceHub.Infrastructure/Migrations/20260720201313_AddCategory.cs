using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "jobs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_jobs_category_id",
                table: "jobs",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_jobs_categories_category_id",
                table: "jobs",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "category_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jobs_categories_category_id",
                table: "jobs");

            migrationBuilder.DropIndex(
                name: "IX_jobs_category_id",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "jobs");
        }
    }
}
