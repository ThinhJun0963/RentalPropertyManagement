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

            // Seed Users
            // Seed Users (1 cho mỗi role, với password hash cho "123456")
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Landlord",
                    LastName = "Admin",
                    Email = "landlord@gmail.com",
                    PasswordHash = "$2y$10$tlwzkAKGJOgk8yPycei6O.ZK4XloYY2mj.G91apzS7sJyn1x3vXn2",
                    Role = Enums.UserRole.Landlord,
                    PhoneNumber = "0123456789"
                },
                new User
                {
                    Id = 2,
                    FirstName = "Tenant",
                    LastName = "User",
                    Email = "tenant@gmail.com",
                    PasswordHash = "$2y$10$tlwzkAKGJOgk8yPycei6O.ZK4XloYY2mj.G91apzS7sJyn1x3vXn2",
                    Role = Enums.UserRole.Tenant,
                    PhoneNumber = "0987654321"
                },
                new User
                {
                    Id = 3,
                    FirstName = "Service",
                    LastName = "Provider",
                    Email = "provider@gmail.com",
                    PasswordHash = "$2y$10$tlwzkAKGJOgk8yPycei6O.ZK4XloYY2mj.G91apzS7sJyn1x3vXn2",
                    Role = Enums.UserRole.ServiceProvider,
                    PhoneNumber = "0112233445"
                }
            );

            // Seed Properties (2 mẫu, 1 occupied, 1 available)
            modelBuilder.Entity<Property>().HasData(
                new Property
                {
                    Id = 1,
                    Address = "123 Main St",
                    City = "Hanoi",
                    SquareFootage = 100,
                    MonthlyRent = 10000000,
                    Description = "Apartment in city center",
                    IsOccupied = false
                },
                new Property
                {
                    Id = 2,
                    Address = "456 Elm St",
                    City = "Saigon",
                    SquareFootage = 150,
                    MonthlyRent = 15000000,
                    Description = "House with garden",
                    IsOccupied = true
                }
            );

            // Seed Contracts (1 mẫu Pending giữa Landlord và Tenant, liên kết Property 1)
            modelBuilder.Entity<Contract>().HasData(
                new Contract
                {
                    Id = 1,
                    PropertyId = 1,
                    TenantId = 2, // Tenant ID 2
                    StartDate = new DateTime(2025, 12, 1),
                    EndDate = new DateTime(2026, 12, 1),
                    RentAmount = 10000000,
                    Status = Enums.ContractStatus.Pending
                }
            );

            // Seed MaintenanceRequests (1 mẫu cho Tenant, liên kết Property 1)
            modelBuilder.Entity<MaintenanceRequest>().HasData(
                new MaintenanceRequest
                {
                    Id = 1,
                    TenantId = 2,
                    PropertyId = 1,
                    Description = "Fix leaking roof",
                    Priority = Enums.RequestPriority.High,
                    Status = Enums.RequestStatus.New,
                    SubmittedDate = DateTime.Now,
                    AttachmentUrl = "example.jpg"
                }
            );

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