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
using Project.Tech.Shop.Services.Products.Enitites;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Project.Tech.Shop.Web.Infrastructure.Filter;
using Project.Tech.Shop.Web.Infrastructure;
using CSharpFunctionalExtensions;

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

        public async Task<IActionResult> Users(CancellationToken cancellationToken)
        {
            var usersResult = await _usersRepository.GetAllAsync(cancellationToken);
            if (usersResult.IsSuccess)
            {
                var model = usersResult.Value.Select(u => new UserProfileViewModel
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FirstName = u.FirstName,
                    LastName = u.Surname,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    TwoFactorEnabled = u.SecuritySettings?.TwoFactorEnabled ?? false,
                    Addresses = u.Addresses.Select(a => new AddressViewModel
                    {
                        AddressId = a.AddressId,
                        AddressLine = a.AddressLine,
                        City = a.City,
                        County = a.County,
                        PostCode = a.PostCode,
                        Country = a.Country,
                        IsShippingAddress = a.IsShippingAddress,
                        IsBillingAddress = a.IsBillingAddress
                    }).ToList(),
                    SecuritySettings = new SecuritySettingsViewModel
                    {
                        TwoFactorEnabled = u.SecuritySettings?.TwoFactorEnabled ?? false,
                        SecurityQuestion = u.SecuritySettings?.SecurityQuestion,
                        SecurityAnswerHash = u.SecuritySettings?.SecurityAnswerHash
                    },
                    Preferences = new UserPreferencesViewModel
                    {
                        ReceiveNewsletter = u.Preferences?.ReceiveNewsletter ?? false,
                        PreferredPaymentMethod = u.Preferences?.PreferredPaymentMethod
                    },
                    Activities = u.Activities.Select(ac => new AccountActivityViewModel
                    {
                        ActivityId = ac.ActivityId,
                        Timestamp = ac.Timestamp,
                        ActivityType = ac.ActivityType,
                        Description = ac.Description
                    }).ToList(),
                    RowVersion = u.RowVersion
                }).ToList();

                return View(model);
            }

            this.AddErrorMessage("Could not load users!");
            ModelState.AddModelError("", "Failed to load users.");
            return View(new List<UserProfileViewModel>());
        }

        public IActionResult AddUser()
        {
            return View(new UserProfileViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UserProfileViewModel model, CancellationToken cancellationToken)
        {
            ModelState.Remove("Preferences");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                FirstName = model.FirstName,
                Surname = model.LastName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.passwordReplacement), // Hash the password
                Role = Enum.Parse<Role>(model.Role),
                Status = AccountStatus.Active // Default status
            };

            var result = await _usersRepository.AddUserAsync(user, cancellationToken);
            if (result.IsSuccess)
            {
                this.AddConfirmationMessage("User added successfully!");
                return RedirectToAction("Users");
            }

            this.AddErrorMessage("Could not add user!");
            ModelState.AddModelError("", result.Error.ToString());
            return View(model);
        }

        public async Task<IActionResult> EditUser(Guid id, CancellationToken cancellationToken)
        {
            ModelState.Remove("Preferences");

            var userResult = await _usersRepository.GetByIdAsync(id, cancellationToken);
            if (userResult.IsSuccess)
            {
                var user = userResult.Value;
                var model = new UserProfileViewModel
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.Surname,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    TwoFactorEnabled = user.SecuritySettings?.TwoFactorEnabled ?? false,
                    passwordReplacement = user.PasswordHash,
                    Addresses = user.Addresses.Select(a => new AddressViewModel
                    {
                        AddressId = a.AddressId,
                        AddressLine = a.AddressLine,
                        City = a.City,
                        County = a.County,
                        PostCode = a.PostCode,
                        Country = a.Country,
                        IsShippingAddress = a.IsShippingAddress,
                        IsBillingAddress = a.IsBillingAddress
                    }).ToList(),
                    SecuritySettings = new SecuritySettingsViewModel
                    {
                        TwoFactorEnabled = user.SecuritySettings?.TwoFactorEnabled ?? false,
                        SecurityQuestion = user.SecuritySettings?.SecurityQuestion,
                        SecurityAnswerHash = user.SecuritySettings?.SecurityAnswerHash
                    },
                    Preferences = new UserPreferencesViewModel
                    {
                        ReceiveNewsletter = user.Preferences?.ReceiveNewsletter ?? false,
                        PreferredPaymentMethod = user.Preferences?.PreferredPaymentMethod
                    },
                    Activities = user.Activities.Select(ac => new AccountActivityViewModel
                    {
                        ActivityId = ac.ActivityId,
                        Timestamp = ac.Timestamp,
                        ActivityType = ac.ActivityType,
                        Description = ac.Description
                    }).ToList(),
                    RowVersion = user.RowVersion
                };

                return View(model);
            }

            this.AddErrorMessage("Could not load user to edit!");
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserProfileViewModel model, CancellationToken cancellationToken)
        {
            ModelState.Remove("Preferences");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userResult = await _usersRepository.GetByIdAsync(model.UserId, cancellationToken);
            if (!userResult.IsSuccess)
            {
                return NotFound();
            }

            var user = userResult.Value;
            user.Username = model.Username;
            user.FirstName = model.FirstName;
            user.Surname = model.LastName;
            user.Email = model.Email;
            user.Role = Enum.Parse<Role>(model.Role);

            var updateResult = await _usersRepository.UpdateUserAsync(user, cancellationToken);
            if (updateResult.IsSuccess)
            {
                this.AddConfirmationMessage("User updated!.");
                return RedirectToAction("Users");
            }

            this.AddErrorMessage("Update failed. Please try again.");
            ModelState.AddModelError("", "Update failed. Please try again.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            var result = await _usersRepository.RemoveUserAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                this.AddConfirmationMessage("User deleted!.");
                return RedirectToAction(nameof(Users));
            }

            this.AddErrorMessage("Failed to delete user.");
            ModelState.AddModelError("", "Failed to delete user.");
            return RedirectToAction(nameof(Users));
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

            this.AddErrorMessage("Failed to load products.");
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

            this.AddErrorMessage(result.Error.ToString());
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

            this.AddErrorMessage("Product not found!");
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

            this.AddErrorMessage("Update failed. Please try again.");
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
                this.AddConfirmationMessage("Deleted Product!");
                return RedirectToAction(nameof(Products));
            }

            this.AddErrorMessage("Failed to delete product.");
            ModelState.AddModelError("", "Failed to delete product.");
            return RedirectToAction(nameof(Products));
        }

    }
}
