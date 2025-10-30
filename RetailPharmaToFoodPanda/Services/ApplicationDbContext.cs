using Microsoft.EntityFrameworkCore;
using RetailPharmaToFoodPanda.Models;

namespace RetailPharmaToFoodPanda.Services;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
    {
    }

    public DbSet<StyleSize> StyleSizes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StyleSize>(entity =>
        {
            entity.ToTable("StyleSize");
            entity.HasKey(e => e.sBarcode);
        });
    }
}
