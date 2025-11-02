using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using RetailPharmaToFoodPanda.Interface;
using RetailPharmaToFoodPanda.Models;
using System.IO;

public class GoogleDriveService : IGoogleDriveService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;
    public GoogleDriveService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _env = env;
        _configuration = configuration;
    }

    public bool IsAuthenticated()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("GoogleAccessToken");
        return !string.IsNullOrEmpty(token);
    }

  
    private async Task<DriveService> GetDriveServiceAsync()
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        var accessToken = session?.GetString("GoogleAccessToken");
        var refreshToken = session?.GetString("GoogleRefreshToken");
        var expiresIn = session?.GetString("GoogleExpiresIn");

        if (string.IsNullOrEmpty(accessToken))
            throw new UnauthorizedAccessException("Not authenticated. Please authorize with Google Drive first.");

        // --- Load client secrets from appsettings.json ---
        var googleSection = _configuration.GetSection("Google");
        var secrets = new ClientSecrets
        {
            ClientId = googleSection["ClientId"],
            ClientSecret = googleSection["ClientSecret"]
        };

        // --- Create token response ---
        var tokenResponse = new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresInSeconds = string.IsNullOrEmpty(expiresIn) ? 3600 : long.Parse(expiresIn),
            IssuedUtc = DateTime.UtcNow
        };

        // --- Create flow ---
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = secrets,
            Scopes = new[] { DriveService.Scope.DriveFile }
        });

        // --- Create credential ---
        var credential = new UserCredential(flow, "user", tokenResponse);

        // --- Return Drive service ---
        return new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "RetailPharmaToFoodPanda"
        });
    }
    public async Task<UploadResult> UploadImageAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            throw new ArgumentException("Image file is required");

        // Validate image extension
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Only image files are allowed");

        try
        {
            var driveService = await GetDriveServiceAsync();

            // Generate unique file name
            var shortGuid = Guid.NewGuid().ToString("N").Substring(0, 8);
            var fileName = $"{Path.GetFileNameWithoutExtension(imageFile.FileName)}_{shortGuid}{extension}";
            var folderId = _configuration["GoogleDrive:UploadFolderId"];

            // File metadata
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName,
                Parents = new[] { folderId } 
            };

            Google.Apis.Drive.v3.Data.File uploadedFile;

            using (var stream = imageFile.OpenReadStream())
            {
                var request = driveService.Files.Create(fileMetadata, stream, imageFile.ContentType);
                request.Fields = "id, name";

                var uploadResult = await request.UploadAsync();

                if (uploadResult.Status != Google.Apis.Upload.UploadStatus.Completed)
                    throw new Exception($"Upload failed: {uploadResult.Exception?.Message}");

                uploadedFile = request.ResponseBody;
            }

            // Make file public
            var permission = new Google.Apis.Drive.v3.Data.Permission
            {
                Role = "reader",
                Type = "anyone"
            };

            await driveService.Permissions.Create(permission, uploadedFile.Id).ExecuteAsync();
            var publicUrl = $"https://drive.google.com/uc?export=view&id={uploadedFile.Id}";
            return new UploadResult
            {
                Success = true,
                FileName = publicUrl,
                Message = "Upload successful"
            };
            // Return public URL
           // return $"https://drive.google.com/uc?export=view&id={uploadedFile.Id}";
        }
        catch (Exception ex)
        {
            return new UploadResult
            {
                Success = false,
                Message = $"Failed to upload image: {ex.Message}"
            };
        }
    }
}
