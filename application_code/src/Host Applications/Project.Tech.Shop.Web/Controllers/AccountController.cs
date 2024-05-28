using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Project.Tech.Shop.Web.Models;
using System.Security.Claims;
using Project.Tech.Shop.Web.services;
using Project.Tech.Shop.Services.UsersAccounts.Entities;
using System.Data;

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
            // Assuming a method to get user details by username
            var userDetailsResults = await _getUserProfileUseCase.GetUserProfileByUsernameAsync(username, cancellationToken);

            if (userDetailsResults.IsFailure)
            {
                // Handle the case where user details are not found
                return View("Error");
            }

            return View(userDetailsResults.Value);
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
