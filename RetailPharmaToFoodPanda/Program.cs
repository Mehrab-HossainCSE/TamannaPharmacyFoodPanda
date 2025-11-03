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

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            // ? STEP 1: Get connection string
            var connectionString = builder.Configuration.GetConnectionString("sqlConnection");

            // ? STEP 2: Discover columns BEFORE registering DbContext
            ApplicationDbContext.DiscoverColumns(connectionString);

            // ? STEP 3: Register ApplicationDbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Session configuration (removed duplicate)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            // Register your custom services
            builder.Services.AddScoped<IStyleSizeService, StyleSizeService>();
            builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
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