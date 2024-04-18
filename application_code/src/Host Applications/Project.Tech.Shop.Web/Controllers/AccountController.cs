using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Project.Tech.Shop.Web.Models;
using System.Security.Claims;

namespace Project.Tech.Shop.Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Here, add your logic to verify the user credentials
                bool isAuthenticated = AuthenticateUser(model.Username, model.Password);

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


        private bool AuthenticateUser(string username, string password)
        {
            // Implement your authentication logic here
            // This is just a placeholder function to simulate authentication
            return (username == "admin" && password == "password");
        }
    }

}
