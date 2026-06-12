using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using projekt.Models;
using projekt.ViewModels;
using System.Security.Claims;

namespace projekt.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private const string AdminRole = "Admin";
        private const string UserRole = "User";

        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(IConfiguration configuration, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["GoogleLoginEnabled"] = HasExternalProvider("Authentication:Google:ClientId", "Authentication:Google:ClientSecret");
            ViewData["MicrosoftLoginEnabled"] = HasExternalProvider("Authentication:Microsoft:ClientId", "Authentication:Microsoft:ClientSecret");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home") ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Category = NormalizeCategory(model.Category)
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var targetRole = user.Category == AdminRole ? AdminRole : UserRole;

                await _userManager.AddToRoleAsync(user, targetRole);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home") ?? "/");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            if (!IsExternalProviderEnabled(provider))
            {
                return NotFound();
            }

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl }) ?? Url.Content("~/");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (!string.IsNullOrWhiteSpace(remoteError))
            {
                ModelState.AddModelError(string.Empty, $"External login error: {remoteError}");
                return View("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "External login information could not be loaded.");
                return View("Login");
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home") ?? "/");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email)
                ?? info.Principal.FindFirstValue("email")
                ?? info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "The external provider did not supply an email address.");
                return View("Login");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Category = UserRole
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View("Login");
                }

                await _userManager.AddToRoleAsync(user, UserRole);
            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {
                foreach (var error in addLoginResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("Login");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home") ?? "/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool HasExternalProvider(string clientIdKey, string clientSecretKey)
        {
            return !string.IsNullOrWhiteSpace(_configuration[clientIdKey]) && !string.IsNullOrWhiteSpace(_configuration[clientSecretKey]);
        }

        private bool IsExternalProviderEnabled(string provider)
        {
            return provider switch
            {
                "Google" => HasExternalProvider("Authentication:Google:ClientId", "Authentication:Google:ClientSecret"),
                "Microsoft" => HasExternalProvider("Authentication:Microsoft:ClientId", "Authentication:Microsoft:ClientSecret"),
                _ => false
            };
        }

        private static string NormalizeCategory(string category)
        {
            return string.Equals(category, AdminRole, StringComparison.OrdinalIgnoreCase) ? AdminRole : UserRole;
        }
    }
}