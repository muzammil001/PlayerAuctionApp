using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerAuctionApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    MaximumPlayerCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MaximumSquadSize = table.Column<int>(type: "INTEGER", nullable: false),
                    StartingBudgetPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    CashPurchaseLimit = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuctionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Nationality = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false),
                    BattingHand = table.Column<string>(type: "TEXT", maxLength: 12, nullable: true),
                    BowlingStyle = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    PhotoDataUrl = table.Column<string>(type: "TEXT", nullable: true),
                    BasePricePoints = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuctionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false, collation: "NOCASE"),
                    City = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    LogoDataUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuctionPurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PricePoints = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    PurchasedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionPurchases_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionPurchases_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamOwners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamOwners_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Auctions",
                columns: new[] { "Id", "CashPurchaseLimit", "MaximumPlayerCount", "MaximumSquadSize", "Name", "StartingBudgetPoints" },
                values: new object[] { new Guid("b5dd884c-7729-45aa-bce7-0c91535aa6c9"), 3, 96, 16, "Cricket Player Auction", 100000 });

            migrationBuilder.CreateIndex(
                name: "IX_AuctionPurchases_PlayerId",
                table: "AuctionPurchases",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuctionPurchases_TeamId",
                table: "AuctionPurchases",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_AuctionId",
                table: "Players",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamOwners_TeamId",
                table: "TeamOwners",
                column: "TeamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_AuctionId_Name",
                table: "Teams",
                columns: new[] { "AuctionId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionPurchases");

            migrationBuilder.DropTable(
                name: "TeamOwners");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Auctions");
        }
    }
}
