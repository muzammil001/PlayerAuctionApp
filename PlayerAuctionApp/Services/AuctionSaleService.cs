using System.Data;
using Microsoft.EntityFrameworkCore;
using PlayerAuctionApp.Data;
using PlayerAuctionApp.Domain;

namespace PlayerAuctionApp.Services;

public sealed class AuctionSaleService(IDbContextFactory<AuctionDbContext> contextFactory)
{
    public async Task<string?> SellPlayerAsync(
        Guid playerId,
        Guid teamId,
        int winningPrice,
        PurchasePaymentMethod paymentMethod,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        var auction = await db.Auctions.SingleAsync(
            item => item.Id == AuctionDbContext.DefaultAuctionId, cancellationToken);
        var player = await db.Players.SingleOrDefaultAsync(
            item => item.Id == playerId && item.AuctionId == auction.Id, cancellationToken);
        if (player is null)
        {
            return "This player is not in the auction pool.";
        }

        if (await db.AuctionPurchases.AnyAsync(purchase => purchase.PlayerId == playerId, cancellationToken))
        {
            return "This player has already been sold.";
        }

        if (winningPrice < player.BasePricePoints)
        {
            return $"The winning price must be at least {player.BasePricePoints:N0} points.";
        }

        var team = await db.Teams.SingleOrDefaultAsync(
            item => item.Id == teamId && item.AuctionId == auction.Id, cancellationToken);
        if (team is null)
        {
            return "Choose a team for this player.";
        }

        var teamPurchases = await db.AuctionPurchases
            .Where(purchase => purchase.TeamId == teamId)
            .ToArrayAsync(cancellationToken);
        if (teamPurchases.Length >= auction.MaximumSquadSize)
        {
            return $"{team.Name} already has the maximum of {auction.MaximumSquadSize} players.";
        }

        if (paymentMethod == PurchasePaymentMethod.Cash)
        {
            var cashPurchases = teamPurchases.Count(purchase => purchase.PaymentMethod == PurchasePaymentMethod.Cash);
            if (cashPurchases >= auction.CashPurchaseLimit)
            {
                return $"{team.Name} has used all {auction.CashPurchaseLimit} cash purchases.";
            }
        }
        else
        {
            var pointsSpent = teamPurchases
                .Where(purchase => purchase.PaymentMethod == PurchasePaymentMethod.Points)
                .Sum(purchase => purchase.PricePoints);
            var pointsRemaining = auction.StartingBudgetPoints - pointsSpent;
            if (winningPrice > pointsRemaining)
            {
                return $"{team.Name} only has {pointsRemaining:N0} points remaining.";
            }
        }

        db.AuctionPurchases.Add(new AuctionPurchase
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            PlayerId = playerId,
            PricePoints = winningPrice,
            PaymentMethod = paymentMethod,
            PurchasedAtUtc = DateTime.UtcNow,
        });

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return null;
    }
}
