using Microsoft.EntityFrameworkCore;
using RentalPropertyManagement.DAL.Entities; // Sử dụng Entities từ DAL

namespace RentalPropertyManagement.DAL.Data
{
    public class RentalDbContext : DbContext
    {
        public RentalDbContext(DbContextOptions<RentalDbContext> options)
            : base(options)
        {
        }

        // --- Thêm DbSet cho các Entity Models ---
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<PaymentInvoice> PaymentInvoices { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình cho User (Email là duy nhất)
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // 2. Cấu hình cho MaintenanceRequest
            // Mối quan hệ AssignedProvider (Service Provider)
            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(mr => mr.AssignedProvider)
                .WithMany(u => u.AssignedRequests)
                .HasForeignKey(mr => mr.AssignedProviderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Mối quan hệ Tenant (Người gửi yêu cầu)
            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(mr => mr.Tenant)
                .WithMany(u => u.SubmittedRequests)
                .HasForeignKey(mr => mr.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Cấu hình cho Contract
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Tenant)
                .WithMany(u => u.ContractsAsTenant)
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Cấu hình cho PaymentInvoice
            modelBuilder.Entity<PaymentInvoice>()
                .HasOne(pi => pi.Contract)
                .WithMany()
                .HasForeignKey(pi => pi.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentInvoice>()
                .HasOne(pi => pi.Tenant)
                .WithMany(u => u.PaymentInvoices)
                .HasForeignKey(pi => pi.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. Cấu hình cho Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentInvoice)
                .WithMany()
                .HasForeignKey(p => p.PaymentInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Tenant)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}