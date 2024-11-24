using Microsoft.EntityFrameworkCore;
using ReelTrack.Models;

namespace ReelTrack.Helpers;

public class ReelTrackDbContext : DbContext
{
    public ReelTrackDbContext(DbContextOptions<ReelTrackDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Family> Families => Set<Family>();
    public DbSet<WatchList> WatchLists => Set<WatchList>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieWatch> MovieWatches => Set<MovieWatch>();
    public DbSet<WatchOrder> WatchOrders => Set<WatchOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Family)
            .WithMany(f => f.Members)
            .HasForeignKey(u => u.FamilyId)
            .IsRequired(false);

        modelBuilder.Entity<WatchList>()
            .HasMany(w => w.Members)
            .WithMany(u => u.WatchLists);

        modelBuilder.Entity<MovieWatch>()
            .HasOne(mw => mw.ChoosenBy)
            .WithMany()
            .HasForeignKey(mw => mw.ChoosenById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}