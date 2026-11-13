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
        var auction = await db.Auctions.AsNoTracking()
            .Include(item => item.Teams).ThenInclude(team => team.Owner)
            .Include(item => item.Teams).ThenInclude(team => team.Purchases)
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
                team.Purchases.Count))
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
}
