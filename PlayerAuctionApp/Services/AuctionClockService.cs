using Microsoft.EntityFrameworkCore;
using PlayerAuctionApp.Data;

namespace PlayerAuctionApp.Services;

public sealed class AuctionClockService(IDbContextFactory<AuctionDbContext> contextFactory)
{
    public async Task<DateTime?> GetStartedAtUtcAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.Auctions.AsNoTracking()
            .Where(auction => auction.Id == AuctionDbContext.DefaultAuctionId)
            .Select(auction => auction.StartedAtUtc)
            .SingleAsync(cancellationToken);
    }

    public async Task<DateTime> StartAuctionAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var auction = await db.Auctions.SingleAsync(
            item => item.Id == AuctionDbContext.DefaultAuctionId, cancellationToken);
        auction.StartedAtUtc ??= DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return auction.StartedAtUtc.Value;
    }
}
