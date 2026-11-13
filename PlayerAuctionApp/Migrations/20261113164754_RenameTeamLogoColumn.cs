using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerAuctionApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameTeamLogoColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoDataUrl",
                table: "Teams",
                newName: "LogoImage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoImage",
                table: "Teams",
                newName: "LogoDataUrl");
        }
    }
}
