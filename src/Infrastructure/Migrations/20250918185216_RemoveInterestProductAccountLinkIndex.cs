using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInterestProductAccountLinkIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InterestProductAccountLinks_AccountId",
                table: "InterestProductAccountLinks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InterestProductAccountLinks_AccountId",
                table: "InterestProductAccountLinks",
                column: "AccountId",
                unique: true,
                filter: "\"IsDeleted\" = FALSE");
        }
    }
}
