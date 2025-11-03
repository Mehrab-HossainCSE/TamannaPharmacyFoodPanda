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
           
            var query = _context.StyleSizes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
               
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

           
            product.BufferQty = model.BufferQty;
            product.ECProduct = model.ECProduct;
            product.ImagePath = model.ImagePath;

           
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
                ErrorMessage = ex.Message 
            };
        }
    }
}