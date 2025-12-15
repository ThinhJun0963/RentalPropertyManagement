using Microsoft.EntityFrameworkCore;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.BLL.Services;
using Microsoft.Extensions.Hosting;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.BLL.Services;
using RentalPropertyManagement.DAL.Data;
using RentalPropertyManagement.DAL.Interfaces;
using RentalPropertyManagement.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies; // Cần thêm using này

var builder = WebApplication.CreateBuilder(args);

// --- 1. CẤU HÌNH AUTHENTICATION (BẮT BUỘC) ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Trang Login sẽ là trang mặc định khi chưa đăng nhập
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });
builder.Services.AddAuthorization();
// ----------------------------------------------


// Cấu hình DbContext (Giữ nguyên)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<RentalDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


// Đăng ký Repositories & Services (Giữ nguyên)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();


// Add services to the container.
builder.Services.AddRazorPages();
            // Đăng ký UnitOfWork (Scoped - mỗi request một instance)
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Đăng ký các Service của BLL (mẫu cho Contract)
            builder.Services.AddScoped<IContractService, ContractService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- 2. SỬ DỤNG MIDDLEWARE (BẮT BUỘC) ---
// Phải đặt UseAuthentication và UseAuthorization sau UseRouting
app.UseAuthentication();
app.UseAuthorization();
// ------------------------------------------

app.MapRazorPages();

app.Run();