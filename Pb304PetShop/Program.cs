using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pb304PetShop.DataContext;
using Pb304PetShop.DataContext.Entities;
using Pb304PetShop.MailKitImplementations;
using static Pb304PetShop.Controllers.AccountController;

namespace Pb304PetShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                //.AddErrorDescriber<AzerbaijaniIdentityErrorDescriber>()
                .AddDefaultTokenProviders();
            //builder.WebHost.ConfigureKestrel(options =>
            //{
            //    options.ListenLocalhost(5001, listenOptions =>
            //    {
            //        listenOptions.UseHttps();
            //    });
            //});

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddTransient<IMailService, MailKitMailService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "wishlist",
                pattern: "Wishlist/{action}/{productId}",
                defaults: new { controller = "Wishlist", action = "Index" });
            app.Run();
        }
    }
}
