using RetailPharmaToFoodPanda.Models;

namespace RetailPharmaToFoodPanda.Interface
{
    public interface IGoogleDriveService
    {
        Task<UploadResult> UploadImageAsync(IFormFile imageFile);
        bool IsAuthenticated();
    }
}
