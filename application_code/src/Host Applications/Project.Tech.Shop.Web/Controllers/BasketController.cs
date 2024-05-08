using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Project.Tech.Shop.Services.Products.Enitites;
using Project.Tech.Shop.Services.Products.Repositories;
using Project.Tech.Shop.Web.Models;
using Project.Tech.Shop.Web.services;
using System.Runtime.Intrinsics.Arm;
using System.Threading;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Project.Tech.Shop.Web.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketRepository _basketRepository;
        private readonly GetUserProfileUseCase _getUserProfileUseCase;

        public BasketController(IBasketRepository basketRepository, GetUserProfileUseCase getUserProfileUseCase)
        {
            _basketRepository = basketRepository;
            _getUserProfileUseCase = getUserProfileUseCase;
        }

        public async Task<IActionResult> GetBasketItemCount(CancellationToken cancellationToken)
        {
            if (User.Identity.IsAuthenticated)
            {
                var username = User.Identity.Name;
                var customerId = await _getUserProfileUseCase.GetUserProfileByUsernameAsync(username, cancellationToken);
                var result = await _basketRepository.GetItemCountAsync(customerId.Value.UserId);
                return Json(new { count = result.Value });
            }
            return Json(new { count = 0 });
        }


        [HttpPost]
        public async Task<IActionResult> AddToBasket(int productId, CancellationToken cancellationToken, int quantity = 1)
        {
            var username = User.Identity.Name;

            // Assuming a method to get user details by username
            var userDetailsResults = await _getUserProfileUseCase.GetUserProfileByUsernameAsync(username, cancellationToken);
            if (userDetailsResults.IsFailure)
            {
                // Handle the case where user details are not found
                return View("Error");
            }


            var customerId = userDetailsResults.Value.UserId; // Implement this method based on your session or authentication system.
            var result = await _basketRepository.AddItemAsync(customerId, productId, quantity);

            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "Product added to basket!" });
            }
            else
            {
                return Json(new { success = false, message = result.Error });
            }
        }

        public async Task<IActionResult> ViewBasket(CancellationToken cancellationToken)
        {
            var username = User.Identity.Name;
            var userDetailsResult = await _getUserProfileUseCase.GetUserProfileByUsernameAsync(username, cancellationToken);

            if (userDetailsResult.IsFailure)
            {
                return View("Error"); // Or redirect to an error page
            }

            var customerId = userDetailsResult.Value.UserId;
            var basket = await _basketRepository.GetActiveBasketByCustomerIdAsync(customerId);

            if (basket.IsFailure)
            {
                return View("Error"); // Or handle empty basket scenario
            }

            var viewModel = new BasketViewModel
            {
                Items = basket.Value.Items.Select(item => new BasketViewModel.BasketItemViewModel
                {
                    BasketItemId = item.BasketItemId,
                    ProductId = item.ProductId,
                    Product = new ProductViewModel(){
                        ProductId = item.Product.ProductId,
                        Name = item.Product.Name,
                        Condition = (Condition)item.Product.Condition,
                        Brand = item.Product.Brand,
                        Series = item.Product.Series,
                        ProcessorType = item.Product.ProcessorType,
                        RAM = item.Product.RAM,
                        Storage = item.Product.Storage,
                        StorageType = item.Product.StorageType,
                        GraphicsCard = item.Product.GraphicsCard,
                        ScreenSize = item.Product.ScreenSize,
                        TouchScreen = item.Product.TouchScreen,
                        Description = item.Product.Description,
                        Price = item.Product.Price,
                        Category = (Category)item.Product.Category,
                        StockLevel = item.Product.StockLevel,
                        Image = item.Product.Image
                    },
                    Price = item.Product.Price,
                    Quantity = item.Quantity
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int productId, int basketItemId, CancellationToken cancellationToken)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var username = User.Identity.Name;
            var userDetailsResult = await _getUserProfileUseCase.GetUserProfileByUsernameAsync(username, cancellationToken);

            if (userDetailsResult.IsFailure)
            {
                return View("Error"); // Or redirect to an error page
            }

            // Assuming a method RemoveItemAsync exists in your repository
            var result = await _basketRepository.RemoveItemAsync(userDetailsResult.Value.UserId, productId, basketItemId);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "Item removed successfully." });
            }
            else
            {
                return Json(new { success = false, message = result.Error });
            }
            
        }

    }
}
