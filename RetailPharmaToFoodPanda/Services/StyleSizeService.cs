using Microsoft.EntityFrameworkCore;
using RetailPharmaToFoodPanda.Interface;
using RetailPharmaToFoodPanda.Models;

namespace RetailPharmaToFoodPanda.Services;

public class StyleSizeService : IStyleSizeService
{
    private readonly ApplicationDbContext _context;

    public StyleSizeService(ApplicationDbContext context)
    {
        _context = context;

    }

    public async Task<StyleSizeSearchResult> SearchProductsAsync(string searchQuery)
    {
        try
        {
            // Start from base query (no tracking for performance)
            var query = _context.StyleSizes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                // Use SQL LIKE behavior — EF Core translates .Contains() into SQL LIKE '%value%'
                // Convert to lowercase for case-insensitive match
                string loweredQuery = searchQuery.ToLower();

                query = query.Where(p =>
                   EF.Functions.Like(p.BTName.ToLower(), $"{loweredQuery}%")

                );
            }

            var styleSizes = await query
                .OrderBy(p => p.PrdName)
                
                .ToListAsync();

            return new StyleSizeSearchResult
            {
                StyleSizes = styleSizes,
                TotalProducts = styleSizes.Count,
                SearchQuery = searchQuery ?? string.Empty
            };
        }
        catch (Exception ex)
        {

            return new StyleSizeSearchResult();
        }
    }

    
    public async Task<bool> UpdateProductAsync(ProductUpdateModel model)
    {
        try
        {
            var product = await _context.StyleSizes.FindAsync(model.sBarcode);
            if (product == null) return false;

            // ✅ Update values
            product.BufferQty = model.BufferQty;
            product.ECProduct = model.ECProduct;
            product.ImagePath = model.ImagePath;

            // ✅ Save changes
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] UpdateProductAsync failed: {ex.Message}");
            return false;
        }
    }

    public async Task<StyleSizeSearchResult> GetAllProduct()
    {
        try
        {
            // Fetch only EC-synced products
            var styleSizes = await _context.StyleSizes
                .AsNoTracking()
                .Where(s => s.ECProduct == true)
                .OrderBy(s => s.PrdName)
                .Take(50)
                .ToListAsync();

            return new StyleSizeSearchResult
            {
                StyleSizes = styleSizes,
                TotalProducts = styleSizes.Count,
                SearchQuery = string.Empty
            };
        }
        catch (Exception ex)
        {

            return new StyleSizeSearchResult
            {
                StyleSizes = new List<StyleSize>(),
                TotalProducts = 0,
                SearchQuery = string.Empty,
                ErrorMessage = ex.Message // <-- send exception message to the view
            };
        }
    }
}