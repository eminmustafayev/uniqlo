
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using uniqilo.DataAcces;
using uniqilo.Extension;
using uniqilo.Helpers;
using uniqilo.Models;

namespace uniqilo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
            });
            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 3;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Lockout.MaxFailedAccessAttempts = 1;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();

            SmtpOptions opt = new();
            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });

            var app = builder.Build();

            app.UseStaticFiles();
            app.MapControllerRoute(name: "register", pattern: "register", defaults: new { controller = "Account", action = "Register" });

            app.MapControllerRoute(name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            app.UseStaticFiles();

            app.Run();
        }
    }
}
