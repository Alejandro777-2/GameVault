using GameVault.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<TradeOffer> TradeOffers => Set<TradeOffer>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Asset>(entity =>
        {
            entity.HasOne(a => a.Owner)
                .WithMany(u => u.Assets)
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<TradeOffer>(entity =>
        {
            entity.HasOne(t => t.Asset)
                .WithMany(a => a.TradeOffers)
                .HasForeignKey(t => t.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Owner)
                .WithMany(u => u.TradeOffers)
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Review>(entity =>
        {
            // Both FKs must be Restrict to avoid multiple cascade paths
            entity.HasOne(r => r.FromUser)
                .WithMany(u => u.ReviewsGiven)
                .HasForeignKey(r => r.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.ToUser)
                .WithMany(u => u.ReviewsReceived)
                .HasForeignKey(r => r.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<WishlistItem>(entity =>
        {
            entity.HasOne(w => w.User)
                .WithMany(u => u.WishlistItems)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(w => w.Asset)
                .WithMany()
                .HasForeignKey(w => w.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(w => new { w.UserId, w.AssetId }).IsUnique();
        });
    }
}
