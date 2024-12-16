
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using uniqilo.DataAcces;
using uniqilo.Helpers;
using uniqilo.Models;

namespace uniqilo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
            });
            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 3;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Lockout.MaxFailedAccessAttempts = 1;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();
            SmtpOptions options = new SmtpOptions();
            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
            builder.Services.AddControllersWithViews();
            var app = builder.Build();
            app.UseStaticFiles();

            app.UseStaticFiles();
            app.MapControllerRoute(name: "register", pattern: "register", defaults: new { controller = "Account", action = "Register" });


            app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
          );
            app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

             
             
            app.Run();
        }
    }
}
