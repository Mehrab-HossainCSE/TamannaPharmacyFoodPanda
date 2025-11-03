using RetailPharmaToFoodPanda.Models;

namespace RetailPharmaToFoodPanda.Interface
{
    public interface IStyleSizeService
    {
        Task<StyleSizeSearchResult> SearchProductsAsync(string searchQuery);
       
        Task<bool> UpdateProductAsync(ProductUpdateModel model);
        Task<StyleSizeSearchResult> GetAllProduct();
    }
}
