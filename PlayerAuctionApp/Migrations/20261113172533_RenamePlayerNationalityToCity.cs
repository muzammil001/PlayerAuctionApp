using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerAuctionApp.Migrations
{
    /// <inheritdoc />
    public partial class RenamePlayerNationalityToCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nationality",
                table: "Players",
                newName: "City");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "City",
                table: "Players",
                newName: "Nationality");
        }
    }
}
