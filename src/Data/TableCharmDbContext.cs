using Microsoft.EntityFrameworkCore;
using TableCharm.Models;

namespace TableCharm.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for TableCharm application
    /// </summary>
    public class TableCharmDbContext : DbContext
    {
        public TableCharmDbContext(DbContextOptions<TableCharmDbContext> options)
            : base(options)
        {
        }

        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Commission> Commissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Distributor entity
            modelBuilder.Entity<Distributor>()
                .HasOne(d => d.ParentDistributor)
                .WithMany(d => d.DirectDownline)
                .HasForeignKey(d => d.ParentDistributorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Distributor>()
                .HasMany(d => d.Sales)
                .WithOne(s => s.Distributor)
                .HasForeignKey(s => s.DistributorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Distributor>()
                .HasMany(d => d.Commissions)
                .WithOne(c => c.Distributor)
                .HasForeignKey(c => c.DistributorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes for performance
            modelBuilder.Entity<Sale>()
                .HasIndex(s => new { s.DistributorId, s.SaleDate })
                .HasDatabaseName("idx_sales_distributor_date");

            modelBuilder.Entity<Commission>()
                .HasIndex(c => new { c.DistributorId, c.StartDate })
                .HasDatabaseName("idx_commission_distributor_date");

            modelBuilder.Entity<Distributor>()
                .HasIndex(d => d.Email)
                .IsUnique()
                .HasDatabaseName("idx_distributor_email");
        }
    }
}
