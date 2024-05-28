using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Project.Tech.Shop.Web.Infastructure.Filter
{
    public class AdminAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // If the user is not authenticated, redirect to the login page
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
            // If the user is authenticated but not an admin, redirect to the home page
            else if (!user.IsInRole("Admin"))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
