using Microsoft.EntityFrameworkCore;

namespace RentalPropertyManagement.DAL.Data
{
    public class RentalDbContext : DbContext
    {
        public RentalDbContext(DbContextOptions<RentalDbContext> options)
            : base(options)
        {
        }
        // Thêm các DbSet ở đây

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình quan hệ, fluent API:
            base.OnModelCreating(modelBuilder);
        }
    }
}