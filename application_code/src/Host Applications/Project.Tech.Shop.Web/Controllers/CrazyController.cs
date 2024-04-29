using Microsoft.AspNetCore.Mvc;

namespace Project.Tech.Shop.Web.Controllers
{
    public class CrazyController : Controller
    {
        private readonly ILogger<CrazyController> _logger;
        public CrazyController(ILogger<CrazyController> logger)
        {
            _logger = logger;
        }

        public ActionResult SimpleGame()
        {
            return View();
        }
    }
}
