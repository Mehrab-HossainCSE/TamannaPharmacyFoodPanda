using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace RetailPharmaToFoodPanda.Controllers
{
    public class GoogleDriveController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        public GoogleDriveController(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var isAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("GoogleAccessToken"));
            ViewBag.IsAuthenticated = isAuthenticated;
            return View();
        }

        //[HttpGet]
        //public IActionResult Authorize(string returnUrl = null)
        //{
        //    // Store return URL in session
        //    if (!string.IsNullOrEmpty(returnUrl))
        //    {
        //        HttpContext.Session.SetString("GoogleDriveReturnUrl", returnUrl);
        //    }

        //    var credentialsPath = Path.Combine(_env.WebRootPath, "Credentials.json");

        //    if (!System.IO.File.Exists(credentialsPath))
        //        return Content("Credentials.json not found in wwwroot folder!");

        //    var secrets = GoogleClientSecrets.FromStream(
        //        new FileStream(credentialsPath, FileMode.Open, FileAccess.Read)
        //    ).Secrets;

        //    var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //    {
        //        ClientSecrets = secrets,
        //        Scopes = new[] { DriveService.Scope.DriveFile }
        //    });

        //    string redirectUri = Url.Action("OAuthCallback", "GoogleDrive", null, Request.Scheme);
        //    var authorizationUrl = flow.CreateAuthorizationCodeRequest(redirectUri).Build();

        //    return Redirect(authorizationUrl.ToString());
        //}

        //[HttpGet]
        //public async Task<IActionResult> OAuthCallback(string code, string error)
        //{
        //    if (!string.IsNullOrEmpty(error))
        //    {
        //        TempData["Error"] = $"Authorization failed: {error}";
        //        return RedirectToAction("Index");
        //    }

        //    if (string.IsNullOrEmpty(code))
        //    {
        //        TempData["Error"] = "Authorization code is missing";
        //        return RedirectToAction("Index");
        //    }

        //    try
        //    {
        //        var credentialsPath = Path.Combine(_env.WebRootPath, "Credentials.json");
        //        var secrets = GoogleClientSecrets.FromStream(
        //            new FileStream(credentialsPath, FileMode.Open, FileAccess.Read)
        //        ).Secrets;

        //        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //        {
        //            ClientSecrets = secrets,
        //            Scopes = new[] { DriveService.Scope.DriveFile }
        //        });

        //        string redirectUri = Url.Action("OAuthCallback", "GoogleDrive", null, Request.Scheme);
        //        var token = await flow.ExchangeCodeForTokenAsync("user", code, redirectUri, CancellationToken.None);

        //        // Store token in session
        //        HttpContext.Session.SetString("GoogleAccessToken", token.AccessToken);

        //        if (!string.IsNullOrEmpty(token.RefreshToken))
        //            HttpContext.Session.SetString("GoogleRefreshToken", token.RefreshToken);

        //        if (token.ExpiresInSeconds.HasValue)
        //            HttpContext.Session.SetString("GoogleExpiresIn", token.ExpiresInSeconds.Value.ToString());

        //        TempData["Success"] = "Successfully connected to Google Drive!";

        //        // Redirect back to the page user came from
        //        var returnUrl = HttpContext.Session.GetString("GoogleDriveReturnUrl");
        //        HttpContext.Session.Remove("GoogleDriveReturnUrl");

        //        if (!string.IsNullOrEmpty(returnUrl))
        //            return Redirect(returnUrl);

        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = $"Authorization error: {ex.Message}";
        //        return RedirectToAction("Index");
        //    }
        //}
        [HttpGet]
        public IActionResult Authorize(string returnUrl = null)
        {
            // Store return URL in session
            if (!string.IsNullOrEmpty(returnUrl))
                HttpContext.Session.SetString("GoogleDriveReturnUrl", returnUrl);

            // --- Read client secrets from appsettings.json ---
            var googleSection = _configuration.GetSection("Google");
            var secrets = new ClientSecrets
            {
                ClientId = googleSection["ClientId"],
                ClientSecret = googleSection["ClientSecret"]
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = new[] { DriveService.Scope.DriveFile }
            });

            string redirectUri = Url.Action("OAuthCallback", "GoogleDrive", null, Request.Scheme);
            var authorizationUrl = flow.CreateAuthorizationCodeRequest(redirectUri).Build();

            return Redirect(authorizationUrl.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> OAuthCallback(string code, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                TempData["Error"] = $"Authorization failed: {error}";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(code))
            {
                TempData["Error"] = "Authorization code is missing";
                return RedirectToAction("Index");
            }

            try
            {
                // --- Read client secrets from appsettings.json ---
                var googleSection = _configuration.GetSection("Google");
                var secrets = new ClientSecrets
                {
                    ClientId = googleSection["ClientId"],
                    ClientSecret = googleSection["ClientSecret"]
                };

                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = secrets,
                    Scopes = new[] { DriveService.Scope.DriveFile }
                });

                string redirectUri = Url.Action("OAuthCallback", "GoogleDrive", null, Request.Scheme);
                var token = await flow.ExchangeCodeForTokenAsync("user", code, redirectUri, CancellationToken.None);

                // Store token in session
                HttpContext.Session.SetString("GoogleAccessToken", token.AccessToken);

                if (!string.IsNullOrEmpty(token.RefreshToken))
                    HttpContext.Session.SetString("GoogleRefreshToken", token.RefreshToken);

                if (token.ExpiresInSeconds.HasValue)
                    HttpContext.Session.SetString("GoogleExpiresIn", token.ExpiresInSeconds.Value.ToString());

                TempData["Success"] = "Successfully connected to Google Drive!";

                // Redirect back to original page
                var returnUrl = HttpContext.Session.GetString("GoogleDriveReturnUrl");
                HttpContext.Session.Remove("GoogleDriveReturnUrl");

                return !string.IsNullOrEmpty(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Authorization error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("GoogleAccessToken");
            HttpContext.Session.Remove("GoogleRefreshToken");
            HttpContext.Session.Remove("GoogleExpiresIn");

            TempData["Success"] = "Logged out from Google Drive";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CheckAuth()
        {
            var isAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("GoogleAccessToken"));
            return Json(new { isAuthenticated });
        }

    }
}