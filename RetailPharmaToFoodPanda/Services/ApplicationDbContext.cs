using Microsoft.EntityFrameworkCore;
using RetailPharmaToFoodPanda.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class ApplicationDbContext : DbContext
{
    private static HashSet<string>? _availableColumns;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<StyleSize> StyleSizes { get; set; }

   
    public static void DiscoverColumns(string connectionString)
    {
        if (_availableColumns != null) return; 

        _availableColumns = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

        using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StyleSize'";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            _availableColumns.Add(reader.GetString(0));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (_availableColumns == null)
        {
            throw new InvalidOperationException("Call ApplicationDbContext.DiscoverColumns() before creating the context.");
        }

        modelBuilder.Entity<StyleSize>(entity =>
        {
            entity.ToTable("StyleSize");
            entity.HasKey(e => e.sBarcode);
            entity.Property(e => e.sBarcode).IsRequired();

            
            var allProps = typeof(StyleSize).GetProperties();
            foreach (var prop in allProps)
            {
                if (prop.Name.Equals(nameof(StyleSize.sBarcode), StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!_availableColumns.Contains(prop.Name))
                {
                    entity.Ignore(prop.Name);
                }
            }
        });
    }
}