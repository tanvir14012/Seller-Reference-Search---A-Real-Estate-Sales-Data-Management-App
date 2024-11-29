using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Seller_Reference_Search.Infrastructure.Data.Models;
using Seller_Reference_Search.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Seller_Reference_Search.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
                return Redirect("/");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> Login(Login loginModel)
        {
            if (_signInManager.IsSignedIn(User))
                return Redirect("/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginModel.Email);
                if(user != null)
                {
                    var signInAttempt = await _signInManager.PasswordSignInAsync(user, loginModel.Password, loginModel.RememberMe, true);

                    if (signInAttempt.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (signInAttempt.IsLockedOut)
                    {
                        ModelState.AddModelError("", "Your account has been locked due to too many failed sign-in attempts. Please try again after some time.");
                        return View(loginModel);
                    }
                }

                ModelState.AddModelError(string.Empty, "Incorrect email or password.");
            }

            return View(loginModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
                return LocalRedirect(returnUrl);
            }

            return Redirect("~/");

        }
    }
}
