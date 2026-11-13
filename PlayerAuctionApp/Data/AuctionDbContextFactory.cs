using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlayerAuctionApp.Data;

public sealed class AuctionDbContextFactory : IDesignTimeDbContextFactory<AuctionDbContext>
{
    public AuctionDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AuctionDbContext>()
            .UseSqlite("Data Source=player-auction.db")
            .Options;

        return new AuctionDbContext(options);
    }
}
