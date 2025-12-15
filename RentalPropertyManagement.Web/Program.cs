using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.BLL.Services;
using RentalPropertyManagement.DAL.Data;
using RentalPropertyManagement.DAL.Interfaces;
using RentalPropertyManagement.DAL.Repositories;

namespace RentalPropertyManagement.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Thêm SignalR để hỗ trợ real-time
            builder.Services.AddSignalR();
                
            // Đăng ký DbContext với SQL Server
            builder.Services.AddDbContext<RentalDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký UnitOfWork (Scoped - mỗi request một instance)
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Đăng ký các Service của BLL (mẫu cho Contract)
            builder.Services.AddScoped<IContractService, ContractService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}