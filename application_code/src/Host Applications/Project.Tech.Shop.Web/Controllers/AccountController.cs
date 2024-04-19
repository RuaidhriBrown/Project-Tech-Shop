using block.chain.services.Transactions.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Tech.Shop.Services.UsersAccounts.Entities;
using Project.Tech.Shop.Web.Models;
using System.Security.Claims;
using BCrypt.Net;
using System.Text;

namespace Project.Tech.Shop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserAccountsRepository _userAccountsRepository;

        public AccountController(
            IUserAccountsRepository userAccounts)
        {
            _userAccountsRepository = userAccounts;
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            
            string password = "testingPassword"; // Choose a strong password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            Console.WriteLine($"Hashed Password: {hashedPassword}");

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
                bool isAuthenticated = await AuthenticateUserAsync(model.Username, model.Password, cancellationToken);

                if (isAuthenticated)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username)
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
            var userDetailsResults = await _userAccountsRepository.GetByUsernameAsync(username, cancellationToken);

            if (userDetailsResults.IsFailure)
            {
                // Handle the case where user details are not found
                return View("Error");
            }

            var userDetails = userDetailsResults.Value;

            var viewModel = new UserProfileViewModel
            {
                Username = userDetails.Username,
                Email = userDetails.Email,
                // Map other required fields
            };

            return View(viewModel);
        }

        private async Task<bool> AuthenticateUserAsync(string username, string password, CancellationToken cancellationToken)
        {
            var passwordHashResult = await _userAccountsRepository.GetPasswordHashByUsernameAsync(username, cancellationToken);
            
            if (passwordHashResult.IsFailure) return false;

            var verificationResult = BCrypt.Net.BCrypt.Verify(password, passwordHashResult.Value);
            return verificationResult;
        }
    }

    // not used \/
    public class UserManager
    {
        private PasswordHasher<User> _passwordHasher;

        public UserManager()
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        public PasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
        }
    }
}
