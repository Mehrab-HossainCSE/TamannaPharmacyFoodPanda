using Microsoft.EntityFrameworkCore;
using RetailPharmaToFoodPanda.Interface;
using RetailPharmaToFoodPanda.Services;

namespace RetailPharmaToFoodPanda
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

           
            var connectionString = builder.Configuration.GetConnectionString("sqlConnection");

           
            ApplicationDbContext.DiscoverColumns(connectionString);

            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddScoped<IStyleSizeService, StyleSizeService>();
            builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService>();

            var app = builder.Build();

           
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Product}/{action=ProductSearch}/{id?}");

            app.Run();
        }
    }
}