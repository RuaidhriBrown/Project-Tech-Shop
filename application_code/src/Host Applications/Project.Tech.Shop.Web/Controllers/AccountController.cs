using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Project.Tech.Shop.Web.Models;
using System.Security.Claims;
using Project.Tech.Shop.Web.services;
using Project.Tech.Shop.Services.UsersAccounts.Entities;
using System.Data;
using Project.Tech.Shop.Web.Infrastructure;

namespace Project.Tech.Shop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountUseCase _accountUseCase;
        private readonly GetUserProfileUseCase _getUserProfileUseCase;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ILogger<AccountController> logger,
            AccountUseCase loginUseCase,
            GetUserProfileUseCase getUserProfileUseCase)
        {
            _logger = logger;
            _accountUseCase = loginUseCase;
            _getUserProfileUseCase = getUserProfileUseCase;
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                // Here, add your logic to verify the user credentials
                var isAuthenticated = await _accountUseCase.AuthenticateUserAsync(model.Username, model.Password, cancellationToken);

                if (isAuthenticated.IsSuccess)
                {
                    var userDetailsResults = await _getUserProfileUseCase.GetUserProfileByUsernameAsync(model.Username, cancellationToken);


                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userDetailsResults.Value.Username),
                        new Claim(ClaimTypes.Role, userDetailsResults.Value.Role),
                        new Claim(ClaimTypes.GivenName, userDetailsResults.Value.FirstName + userDetailsResults.Value.LastName),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth"); // Use the same scheme name you used during login.
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/Profile
        [HttpGet]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var username = User.Identity.Name;
            var userDetailsResults = await _getUserProfileUseCase.GetUserProfileByUsernameAsync(username, cancellationToken);

            if (userDetailsResults.IsFailure)
            {
                this.AddErrorMessage("Could not retrieve user profile details.");
                return View("Error");
            }

            return View(userDetailsResults.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model, CancellationToken cancellationToken)
        {
            // Remove unwanted ModelState entries
            //ModelState.Remove("SecuritySettings");
            //ModelState.Remove("Preferences");
            //ModelState.Remove("IsShippingAddress");
            //ModelState.Remove("IsBillingAddress");

            //if (!ModelState.IsValid)
            //{
            //    this.AddErrorMessage("Please correct the errors in the form.");
            //    return View(model);
            //}

            var updateResult = await _getUserProfileUseCase.UpdateUserProfileAsync(model, cancellationToken);
            if (updateResult.IsSuccess)
            {
                this.AddConfirmationMessage("Profile updated successfully.");
                return RedirectToAction("Profile");
            }

            this.AddErrorMessage(updateResult.Error);
            return View(model);
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View(new RegistrationViewModel());
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Password and Confirm Password do not match.");
                return View(model);
            }

            // Assuming RegisterUserAsync is a method in LoginUseCase that handles user registration
            var result = await _accountUseCase.RegisterUserAsync(model, cancellationToken);

            if (result.IsSuccess)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", result.Error);
                return View(model);
            }
        }
    }
}
