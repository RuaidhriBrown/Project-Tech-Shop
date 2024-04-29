using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.Products.Enitites;
using Project.Tech.Shop.Services.Products.Repositories;
using Project.Tech.Shop.Web.Models;

namespace Project.Tech.Shop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductsRepository _productsRepository;

        public ProductsController(ILogger<ProductsController> logger, IProductsRepository productsRepository)
        {
            _logger = logger;
            _productsRepository = productsRepository;
        }

        public async Task<IActionResult> Index(string? brand, Category? category, decimal? minPrice, decimal? maxPrice, int? minStorage, int? maxStorage)
        {
            var filters = new FilterProductViewModel
            {
                Brand = brand,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinStorage = minStorage,
                MaxStorage = maxStorage
            };
            ViewBag.Filters = filters;

            var productsQuery = _productsRepository.GetAllQueryable(); // Assuming this method exists

            // Apply filters
            productsQuery = ApplyFilters(productsQuery, filters);

            var productsResult = await productsQuery.ToListAsync();

            var viewModel = new List<ProductViewModel>();
            foreach (var product in productsResult) 
            {
                viewModel.Add(new ProductViewModel()
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Condition = product.Condition,
                    Brand = product.Brand,
                    Series = product.Series,
                    ProcessorType = product.ProcessorType,
                    RAM = product.RAM,
                    Storage = product.Storage,
                    StorageType = product.StorageType,
                    GraphicsCard = product.GraphicsCard,
                    ScreenSize = product.ScreenSize,
                    TouchScreen = product.TouchScreen,
                    Description = product.Description,
                    Price = product.Price,
                    Category = product.Category,
                    StockLevel = product.StockLevel,
                    Image = product.Image
                });
            }
            
            return View(viewModel);
        }

        private IQueryable<Product> ApplyFilters(IQueryable<Product> query, FilterProductViewModel filters)
        {
            if (!string.IsNullOrEmpty(filters.Brand))
                query = query.Where(p => p.Brand == filters.Brand);
            if (filters.Category.HasValue)
                query = query.Where(p => p.Category == filters.Category.Value);
            if (filters.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filters.MinPrice.Value);
            if (filters.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filters.MaxPrice.Value);
            if (filters.MinStorage.HasValue)
                query = query.Where(p => p.Storage >= filters.MinStorage.Value);
            if (filters.MaxStorage.HasValue)
                query = query.Where(p => p.Storage <= filters.MaxStorage.Value);

            return query;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id, CancellationToken cancellationToken)
        {
            var productResult = await _productsRepository.GetByIdAsync(id, cancellationToken);
            if (productResult.IsSuccess)
            {
                var viewModel = new ProductViewModel
                {
                    ProductId = productResult.Value.ProductId,
                    Name = productResult.Value.Name,
                    Condition = productResult.Value.Condition,
                    Brand = productResult.Value.Brand,
                    Series = productResult.Value.Series,
                    ProcessorType = productResult.Value.ProcessorType,
                    RAM = productResult.Value.RAM,
                    Storage = productResult.Value.Storage,
                    StorageType = productResult.Value.StorageType,
                    GraphicsCard = productResult.Value.GraphicsCard,
                    ScreenSize = productResult.Value.ScreenSize,
                    TouchScreen = productResult.Value.TouchScreen,
                    Description = productResult.Value.Description,
                    Price = productResult.Value.Price,
                    Category = productResult.Value.Category,
                    StockLevel = productResult.Value.StockLevel,
                    Image = productResult.Value.Image
                };
                return View(viewModel);
            }
            else
            {
                _logger.LogError("Product with ID {ProductId} not found.", id);
                return View("Error"); // Ensure you have an Error view to handle this scenario.
            }
        }
    }
}
