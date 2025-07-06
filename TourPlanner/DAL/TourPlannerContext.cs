using Microsoft.EntityFrameworkCore;

namespace TourPlanner.DAL
{
    public class TourPlannerContext : DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        public TourPlannerContext(DbContextOptions<TourPlannerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tour>()
                .HasMany(t => t.TourLogs)
                .WithOne(tl => tl.Tour)
                .HasForeignKey(tl => tl.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tour>()
                .Property(t => t.TransportType)
                .HasConversion<string>();

            modelBuilder.Entity<Tour>()
                .Property(t => t.Image)
                .IsRequired(false);
        }
    }
}
