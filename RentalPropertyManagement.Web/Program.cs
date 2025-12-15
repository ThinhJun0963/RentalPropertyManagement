using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.BLL.Services;
using RentalPropertyManagement.DAL.Data;
using RentalPropertyManagement.DAL.Interfaces;
using RentalPropertyManagement.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<RentalDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// 2. Cấu hình Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });
builder.Services.AddAuthorization();
builder.Services.AddSignalR();

// 3. Đăng ký Dependency Injection (DI)
// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IContractService, ContractService>();

builder.Services.AddRazorPages(options =>
{
    // Yêu cầu xác thực cho tất cả các Page trừ Login, Register, và Index (nếu Index chỉ là trang chào mừng)
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Register");
    options.Conventions.AllowAnonymousToPage("/Index");
});

var app = builder.Build();

// Configure Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<RentalPropertyManagement.Web.Hubs.MainHub>("/mainHub"); // Đặt endpoint là /mainHub

app.MapRazorPages();

app.Run();