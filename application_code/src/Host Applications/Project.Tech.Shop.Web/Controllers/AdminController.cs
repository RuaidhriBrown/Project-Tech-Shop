using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Project.Tech.Shop.Web.Models;
using System.Security.Claims;
using Project.Tech.Shop.Web.services;
using Project.Tech.Shop.Services.UsersAccounts.Entities;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Project.Tech.Shop.Services.Products.Repositories;
using Project.Tech.Shop.Services.UsersAccounts.Repositories;
using Project.Tech.Shop.Web.Infastructure.Filter;
using Project.Tech.Shop.Services.Products.Enitites;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Project.Tech.Shop.Web.Controllers
{
    [ServiceFilter(typeof(AdminAuthorizationFilter))]
    public class AdminController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        private readonly IProductsRepository _productsRepository;
        private readonly IUserAccountsRepository _usersRepository;

        public AdminController(
            ILogger<AccountController> logger,
            IProductsRepository productsRepository,
            IUserAccountsRepository usersRepository)
        {
            _logger = logger;
            _productsRepository = productsRepository;
            _usersRepository = usersRepository;
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }


        public async Task<IActionResult> Products(CancellationToken cancellationToken)
        {
            var productsResult = await _productsRepository.GetAllAsync(cancellationToken);
            if (productsResult.IsSuccess)
            {
                // Map the List<Product> to List<ProductViewModel>
                var model = productsResult.Value.Select(p => new ProductViewModel
                {
                    // Include additional fields as necessary
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Condition = (Condition)p.Condition,
                    Brand = p.Brand,
                    Series = p.Series,
                    ProcessorType = p.ProcessorType,
                    RAM = p.RAM,
                    Storage = p.Storage,
                    StorageType = p.StorageType,
                    GraphicsCard = p.GraphicsCard,
                    ScreenSize = p.ScreenSize,
                    TouchScreen = p.TouchScreen,
                    Description = p.Description,
                    Price = p.Price,
                    Category = (Category)p.Category,
                    StockLevel = p.StockLevel,
                    Image = p.Image
                }).ToList();

                return View(model);
            }

            ModelState.AddModelError("", "Failed to load products.");
            return View(new List<ProductViewModel>()); // Ensure to pass an empty list of the expected type to the view in case of failure
        }


        public IActionResult AddProduct()
        {
            return View(new ProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductViewModel model, IFormFile image, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Handle the image if it's provided
            if (image != null && image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    image.CopyTo(ms);
                    model.Image = Convert.ToBase64String(ms.ToArray());
                }
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Category = (int)model.Category,
                StockLevel = model.StockLevel,
                Brand = model.Brand,
                Image = model.Image // Assume it's a Base64 string
            };

            var result = await _productsRepository.AddAsync(product, cancellationToken);
            if (result.IsSuccess)
            {
                return RedirectToAction("Products");
            }

            ModelState.AddModelError("", result.Error.ToString());

            return View(model);
        }


        public async Task<IActionResult> EditProduct(int id, CancellationToken cancellationToken)
        {
            var productResult = await _productsRepository.GetByIdAsync(id, cancellationToken);
            if (productResult.IsSuccess)
            {
                var product = productResult.Value;
                var model = new ProductViewModel
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Category = (Category)product.Category,
                    StockLevel = product.StockLevel,
                    Brand = product.Brand,
                    Condition = (Condition)product.Condition,
                    GraphicsCard = product.GraphicsCard,
                    ProcessorType = product.ProcessorType,
                    RAM = product.RAM,
                    Storage = product.Storage,
                    StorageType = product.StorageType,
                    ScreenSize = product.ScreenSize,
                    TouchScreen = product.TouchScreen,
                    Image = product.Image
                };

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var productResult = await _productsRepository.GetByIdAsync(model.ProductId, cancellationToken);
            if (!productResult.IsSuccess)
            {
                return NotFound();
            }

            var product = productResult.Value;
            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Category = (int)model.Category;
            product.StockLevel = model.StockLevel;
            product.Brand = model.Brand;


            // Update the image only if a new one has been provided
            if (!string.IsNullOrEmpty(model.Image))
            {
                product.Image = model.Image;
            }

            var updateResult = await _productsRepository.UpdateAsync(product, cancellationToken);
            if (updateResult.IsSuccess)
            {
                return RedirectToAction("Products");
            }

            ModelState.AddModelError("", "Update failed. Please try again.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var result = await _productsRepository.DeleteAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(Products));
            }

            ModelState.AddModelError("", "Failed to delete product.");
            return RedirectToAction(nameof(Products));
        }

    }
}
