using Microsoft.EntityFrameworkCore;
using PlayerAuctionApp.Domain;

namespace PlayerAuctionApp.Data;

public sealed class AuctionDbContext(DbContextOptions<AuctionDbContext> options) : DbContext(options)
{
    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamOwner> TeamOwners => Set<TeamOwner>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<AuctionPurchase> AuctionPurchases => Set<AuctionPurchase>();

    public static readonly Guid DefaultAuctionId = Guid.Parse("b5dd884c-7729-45aa-bce7-0c91535aa6c9");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auction>(entity =>
        {
            entity.HasKey(auction => auction.Id);
            entity.Property(auction => auction.Name).HasMaxLength(120).IsRequired();
            entity.HasData(new Auction
            {
                Id = DefaultAuctionId,
                Name = "Cricket Player Auction",
                MaximumPlayerCount = 96,
                MaximumSquadSize = 16,
                StartingBudgetPoints = 100_000,
                CashPurchaseLimit = 3,
            });
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(team => team.Id);
            entity.Property(team => team.Name).HasMaxLength(60).UseCollation("NOCASE").IsRequired();
            entity.Property(team => team.City).HasMaxLength(60).IsRequired();
            entity.Property(team => team.LogoImage).HasColumnType("TEXT");
            entity.HasIndex(team => new { team.AuctionId, team.Name }).IsUnique();
            entity.HasOne(team => team.Auction).WithMany(auction => auction.Teams)
                .HasForeignKey(team => team.AuctionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(team => team.Owner).WithOne(owner => owner.Team)
                .HasForeignKey<TeamOwner>(owner => owner.TeamId).OnDelete(DeleteBehavior.Cascade).IsRequired();
        });

        modelBuilder.Entity<TeamOwner>(entity =>
        {
            entity.HasKey(owner => owner.Id);
            entity.Property(owner => owner.FullName).HasMaxLength(80).IsRequired();
            entity.Property(owner => owner.Phone).HasMaxLength(32).IsRequired();
            entity.Property(owner => owner.Email).HasMaxLength(254);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(player => player.Id);
            entity.Property(player => player.FullName).HasMaxLength(100).IsRequired();
            entity.Property(player => player.City).HasMaxLength(60).IsRequired();
            entity.Property(player => player.Role).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(player => player.BattingHand).HasConversion<string>().HasMaxLength(12);
            entity.Property(player => player.BowlingStyle).HasMaxLength(60);
            entity.Property(player => player.PhotoDataUrl).HasColumnType("TEXT");
            entity.HasOne(player => player.Auction).WithMany(auction => auction.Players)
                .HasForeignKey(player => player.AuctionId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AuctionPurchase>(entity =>
        {
            entity.HasKey(purchase => purchase.Id);
            entity.Property(purchase => purchase.PaymentMethod).HasConversion<string>().HasMaxLength(12);
            entity.HasIndex(purchase => purchase.PlayerId).IsUnique();
            entity.HasOne(purchase => purchase.Team).WithMany(team => team.Purchases)
                .HasForeignKey(purchase => purchase.TeamId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(purchase => purchase.Player).WithOne(player => player.Purchase)
                .HasForeignKey<AuctionPurchase>(purchase => purchase.PlayerId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
