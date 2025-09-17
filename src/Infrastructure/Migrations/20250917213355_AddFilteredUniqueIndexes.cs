using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFilteredUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jobs_JobName",
                table: "Jobs");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobName",
                table: "Jobs",
                column: "JobName",
                unique: true,
                filter: "\"IsDeleted\" = FALSE");

            migrationBuilder.CreateIndex(
                name: "IX_InterestProductAccountLinks_AccountId",
                table: "InterestProductAccountLinks",
                column: "AccountId",
                unique: true,
                filter: "\"IsDeleted\" = FALSE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jobs_JobName",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_InterestProductAccountLinks_AccountId",
                table: "InterestProductAccountLinks");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobName",
                table: "Jobs",
                column: "JobName",
                unique: true);
        }
    }
}
