using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurrantProject.Context;
using RestaurrantProject.Models;


namespace RestaurrantProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<MyContext>(options =>
            options.UseSqlServer("Data Source=.;Initial Catalog=Restaurant;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True"));
            builder.Services.AddHostedService<OrderStatusBackgroundService>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 5;
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<MyContext>();
            builder.Services.AddSession();


            var app = builder.Build();
            app.UseSession();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
