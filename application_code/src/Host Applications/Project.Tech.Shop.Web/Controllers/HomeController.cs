using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.Products.Enitites;
using Project.Tech.Shop.Services.Products.Repositories;
using Project.Tech.Shop.Web.Models;
using System.Diagnostics;

namespace Project.Tech.Shop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductsRepository _productsRepository;

        public HomeController(ILogger<HomeController> logger, IProductsRepository productsRepository)
        {
            _logger = logger;
            _productsRepository = productsRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var allProducts = await _productsRepository.GetAllQueryable().ToListAsync();
            var randomProduct = allProducts.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            var homeViewModel = new HomeViewModel
            {
                RandomProduct = new ProductViewModel
                {
                    ProductId = randomProduct.ProductId,
                    Name = randomProduct.Name,
                    Description = randomProduct.Description,
                    Price = randomProduct.Price,
                    Image = randomProduct.Image,
                    // Add other properties as needed
                },
                News = "Lorem ipsum dolor sit amet consectetur, adipisicing elit. Cumque sed quas tenetur tempore atque fugiat, distinctio unde ab corporis ad velit tempora facere dolorem expedita adipisci reiciendis voluptatibus dolores qui, illum similique iusto quos! Fugit culpa accusamus quaerat, at mollitia quisquam magni obcaecati sapiente illo doloribus excepturi accusantium expedita quidem autem. Fugit totam ipsum eius facere ad ratione, inventore nihil cum, nobis ducimus autem laborum adipisci? Minima inventore mollitia, quos quibusdam similique corporis. Expedita officiis eligendi dicta suscipit laboriosam reprehenderit eveniet illo incidunt animi ipsa aperiam doloribus excepturi impedit, tenetur numquam facere quas laborum. Officia maxime at veritatis iste deserunt. "
            };

            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
