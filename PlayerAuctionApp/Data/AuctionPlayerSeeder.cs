using Microsoft.EntityFrameworkCore;
using PlayerAuctionApp.Domain;

namespace PlayerAuctionApp.Data;

public static class AuctionPlayerSeeder
{
    private static readonly string[] FirstNames =
    [
        "Ahsan", "Bilal", "Danish", "Fahad", "Hamza", "Imran", "Kamran", "Mohammad",
        "Nasir", "Owais", "Rizwan", "Saad", "Talha", "Usman", "Waqas", "Zain",
    ];

    private static readonly string[] LastNames = ["Ahmed", "Ali", "Bashir", "Farooq", "Hussain", "Iqbal"];

    private static readonly string[] Cities =
    [
        "Karachi", "Lahore", "Islamabad", "Rawalpindi", "Peshawar", "Quetta", "Multan",
        "Faisalabad", "Sialkot", "Gujranwala", "Hyderabad", "Bahawalpur", "Sargodha", "Abbottabad",
    ];

    private static readonly string[] BowlingStyles =
    [
        "Right-arm fast", "Left-arm fast", "Right-arm medium", "Left-arm medium",
        "Right-arm off-break", "Right-arm leg-break", "Left-arm orthodox", "Left-arm wrist spin",
    ];

    public static async Task SeedAsync(AuctionDbContext db, CancellationToken cancellationToken = default)
    {
        var auction = await db.Auctions.SingleAsync(
            item => item.Id == AuctionDbContext.DefaultAuctionId, cancellationToken);

        if (await db.Players.AnyAsync(player => player.AuctionId == auction.Id, cancellationToken))
        {
            return;
        }

        var players = new List<Player>(auction.MaximumPlayerCount);
        for (var index = 0; index < auction.MaximumPlayerCount; index++)
        {
            var role = (PlayerRole)(index % Enum.GetValues<PlayerRole>().Length);
            var isBowler = role is PlayerRole.Bowler or PlayerRole.AllRounder;

            players.Add(new Player
            {
                Id = Guid.NewGuid(),
                AuctionId = auction.Id,
                FullName = $"{FirstNames[index % FirstNames.Length]} {LastNames[index / FirstNames.Length]}",
                DateOfBirth = new DateOnly(1990 + index % 16, 1 + index % 12, 1 + index % 27),
                City = Cities[index % Cities.Length],
                Role = role,
                BattingHand = index % 3 == 0 ? BattingHand.Left : BattingHand.Right,
                BowlingStyle = isBowler ? BowlingStyles[index % BowlingStyles.Length] : null,
                BasePricePoints = 5_000 + index % 8 * 2_500,
                PhotoDataUrl = null,
            });
        }

        db.Players.AddRange(players);
        await db.SaveChangesAsync(cancellationToken);
    }
}
