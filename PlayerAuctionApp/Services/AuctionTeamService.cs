using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using PlayerAuctionApp.Data;
using PlayerAuctionApp.Domain;
using PlayerAuctionApp.Models;

namespace PlayerAuctionApp.Services;

public sealed class AuctionTeamService(IDbContextFactory<AuctionDbContext> contextFactory)
{
    public async Task<IReadOnlyList<TeamSummary>> GetTeamsAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var auction = await db.Auctions.AsNoTracking().AsSplitQuery()
            .Include(item => item.Teams).ThenInclude(team => team.Owner)
            .Include(item => item.Teams).ThenInclude(team => team.Purchases).ThenInclude(purchase => purchase.Player)
            .SingleAsync(item => item.Id == AuctionDbContext.DefaultAuctionId, cancellationToken);

        return auction.Teams
            .OrderBy(team => team.Name)
            .Select(team => new TeamSummary(
                team.Id,
                team.Name,
                team.City,
                team.LogoImage,
                team.Owner.FullName,
                team.Owner.Phone,
                team.Owner.Email,
                auction.StartingBudgetPoints - team.Purchases
                    .Where(purchase => purchase.PaymentMethod == PurchasePaymentMethod.Points)
                    .Sum(purchase => purchase.PricePoints),
                auction.CashPurchaseLimit - team.Purchases
                    .Count(purchase => purchase.PaymentMethod == PurchasePaymentMethod.Cash),
                team.Purchases.Count,
                team.Purchases.OrderByDescending(purchase => purchase.PurchasedAtUtc)
                    .Select(purchase => new TeamPlayerSummary(
                        purchase.Player.Id,
                        purchase.Player.FullName,
                        purchase.Player.City,
                        purchase.Player.Role.ToString(),
                        purchase.Player.PhotoDataUrl,
                        purchase.PricePoints,
                        purchase.PaymentMethod.ToString(),
                        purchase.PurchasedAtUtc))
                    .ToArray()))
            .ToArray();
    }

    public async Task<string?> CreateTeamAsync(NewTeamInput input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.City)
            || string.IsNullOrWhiteSpace(input.OwnerName) || string.IsNullOrWhiteSpace(input.OwnerPhone)
            || string.IsNullOrWhiteSpace(input.LogoDataUrl))
        {
            return "Team name, city, team logo, owner name, and owner phone are required.";
        }

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var auction = await db.Auctions.SingleAsync(
            item => item.Id == AuctionDbContext.DefaultAuctionId, cancellationToken);
        var name = input.Name.Trim();

        if (await db.Teams.AnyAsync(team => team.AuctionId == auction.Id && team.Name == name, cancellationToken))
        {
            return "A team with this name already exists.";
        }

        if (!string.IsNullOrEmpty(input.LogoDataUrl) && input.LogoDataUrl.Length > 2_800_000)
        {
            return "The team logo is too large. Choose an image up to 2 MB.";
        }

        var team = new Team
        {
            Id = Guid.NewGuid(),
            AuctionId = auction.Id,
            Name = name,
            City = input.City.Trim(),
            LogoImage = input.LogoDataUrl,
            Owner = new TeamOwner
            {
                Id = Guid.NewGuid(),
                FullName = input.OwnerName.Trim(),
                Phone = input.OwnerPhone.Trim(),
                Email = string.IsNullOrWhiteSpace(input.OwnerEmail) ? null : input.OwnerEmail.Trim(),
            },
        };

        db.Teams.Add(team);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return null;
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqliteException
        {
            SqliteExtendedErrorCode: 2067,
        })
        {
            return "A team with this name already exists.";
        }
    }

    public async Task<string?> UpdateTeamAsync(Guid id, NewTeamInput input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.City) || string.IsNullOrWhiteSpace(input.OwnerName) || string.IsNullOrWhiteSpace(input.OwnerPhone) || string.IsNullOrWhiteSpace(input.LogoDataUrl))
            return "Team name, city, team logo, owner name, and owner phone are required.";
        if (input.LogoDataUrl.Length > 2_800_000) return "The team logo is too large. Choose an image up to 2 MB.";
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var team = await db.Teams.Include(t => t.Owner).SingleOrDefaultAsync(t => t.Id == id && t.AuctionId == AuctionDbContext.DefaultAuctionId, cancellationToken);
        if (team is null) return "Team not found.";
        var name = input.Name.Trim();
        if (await db.Teams.AnyAsync(t => t.AuctionId == team.AuctionId && t.Id != id && t.Name == name, cancellationToken)) return "A team with this name already exists.";
        team.Name = name; team.City = input.City.Trim(); team.LogoImage = input.LogoDataUrl;
        team.Owner.FullName = input.OwnerName.Trim(); team.Owner.Phone = input.OwnerPhone.Trim();
        team.Owner.Email = string.IsNullOrWhiteSpace(input.OwnerEmail) ? null : input.OwnerEmail.Trim();
        try { await db.SaveChangesAsync(cancellationToken); return null; }
        catch (DbUpdateException exception) when (exception.InnerException is SqliteException { SqliteExtendedErrorCode: 2067 }) { return "A team with this name already exists."; }
    }

    public async Task<string?> DeleteTeamAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var team = await db.Teams.Include(t => t.Purchases).Include(t => t.Bids).Include(t => t.Owner).SingleOrDefaultAsync(t => t.Id == id && t.AuctionId == AuctionDbContext.DefaultAuctionId, cancellationToken);
        if (team is null) return "Team not found.";
        if (team.Purchases.Count > 0) return "This team has players in its squad and cannot be deleted.";
        if (team.Bids.Count > 0) return "This team has bidding history and cannot be deleted.";
        db.Teams.Remove(team); await db.SaveChangesAsync(cancellationToken); return null;
    }
}
