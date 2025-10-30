using Microsoft.AspNetCore.Mvc;
using RetailPharmaToFoodPanda.Interface;
using RetailPharmaToFoodPanda.Models;

namespace RetailPharmaToFoodPanda.Controllers
{
    public class ProductController : Controller
    {
        private readonly IStyleSizeService _styleSizeService;
        private readonly IGoogleDriveService _googleDriveService;
        public ProductController(IStyleSizeService styleSizeService, IGoogleDriveService googleDriveService)
        {
            _styleSizeService = styleSizeService;
            _googleDriveService = googleDriveService;
        }

        public async Task<IActionResult> ProductSearch(string? searchQuery)
        {
            StyleSizeSearchResult result;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                // 🔍 Search by query
                result = await _styleSizeService.SearchProductsAsync(searchQuery);
            }
            else
            {
                // 📦 Load all products (default view)
                result = await _styleSizeService.GetAllProduct();
            }

            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return Json(new { success = false, message = "No image file selected" });

            try
            {
                // Check if user is authenticated
                if (!_googleDriveService.IsAuthenticated())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Not authenticated. Please authorize with Google Drive first.",
                        requireAuth = true,
                        authUrl = Url.Action("Authorize", "GoogleDrive", new { returnUrl = Request.Headers["Referer"].ToString() })
                    });
                }

                // Upload image
                var imagePath = await _googleDriveService.UploadImageAsync(imageFile);

                if (!imagePath.Success)
                {
                    return Json(new { success = false, message = imagePath.Message ?? "Failed to upload image" });
                }

                return Json(new { success = true, imagePath =imagePath.FileName });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    requireAuth = true,
                    authUrl = Url.Action("Authorize", "GoogleDrive", new { returnUrl = Request.Headers["Referer"].ToString() })
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] ProductUpdateModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ImagePath))
                return Json(new { success = false, message = "Invalid data" });

            try
            {
                // Your update logic here
                // await _styleSizeService.UpdateProductAsync(model);
             var   result = await _styleSizeService.UpdateProductAsync(model);
                if (!result)
                    return Json(new { success = false, message = "Failed to update product." });

                return Json(new { success = true, message = "Product updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }


}
