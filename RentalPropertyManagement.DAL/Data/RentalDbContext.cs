using Microsoft.EntityFrameworkCore;
using RentalPropertyManagement.DAL.Models;

namespace RentalPropertyManagement.DAL.Data
{
    public class RentalDbContext : DbContext
    {
        public RentalDbContext(DbContextOptions<RentalDbContext> options)
            : base(options)
        {
        }
        // Thêm các DbSet ở đây
        public DbSet<Contract> Contracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình quan hệ, fluent API:
            base.OnModelCreating(modelBuilder);
        }
    }
}