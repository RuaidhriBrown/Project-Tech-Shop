using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.Products;
using Project.Tech.Shop.Services.Products.Enitites;

namespace Project.Tech.Shop.Tests.Common
{
    public static class TestProductsContextDatabaseFactory
    {
        public static ProductsContext CreateDefaultTestContext()
        {
            var databaseOptions = TestDatabaseContextFactory<ProductsContext>.CreateDefaultTestContextOption();
            return new TestProductsContext(databaseOptions);
        }

        public static async Task<Product> CreateTestProduct(
    ProductsContext dbContext,
    Fixture fixture,
    bool save = true)
        {
            var product = new Product
            {
                Name = fixture.Create<string>(),
                Description = fixture.Create<string>(),
                Price = fixture.Create<decimal>(),
                Category = (int)fixture.Create<Category>(),
                StockLevel = fixture.Create<int>(),
                Brand = fixture.Create<string>()
            };

            if (product.Category == (int)Category.Laptop)
            {
                product.Series = fixture.Create<string>();
                product.ProcessorType = fixture.Create<string>();
                product.RAM = fixture.Create<int>();
                product.Storage = fixture.Create<int>();
                product.StorageType = fixture.Create<string>();
                product.GraphicsCard = fixture.Create<string>();
                product.ScreenSize = fixture.Create<double>();
                product.TouchScreen = fixture.Create<bool>();
            }

            if (save)
            {
                dbContext.Products.Add(product);
                await dbContext.SaveChangesAsync();

                // Assuming each product immediately results in a sale (not realistic but for the sake of the example)
                var basket = new Basket
                {
                    CustomerId = Guid.NewGuid(), // Simulate a new customer
                    Status = BasketStatus.Completed
                };

                basket.Items.Add(new BasketItem
                {
                    ProductId = product.ProductId,
                    Quantity = fixture.Create<int>()
                });

                dbContext.Baskets.Add(basket);

                var sale = new Sale
                {
                    SaleId = fixture.Create<int>(), // Associating sale with product for testing
                    CustomerId = basket.CustomerId,
                    SaleDate = fixture.Create<DateTime>(),
                    TotalSaleAmount = product.Price * basket.Items.Sum(i => i.Quantity),
                    BasketId = basket.BasketId
                };

                dbContext.Sales.Add(sale);
                await dbContext.SaveChangesAsync();
            }

            return product;
        }

        public static async Task<Sale> CreateTestSale(
    ProductsContext dbContext,
    Fixture fixture,
    bool save = true)
        {
            // Create a product
            var product = new Product
            {
                Name = fixture.Create<string>(),
                Description = fixture.Create<string>(),
                Price = fixture.Create<decimal>(),
                Category = (int)fixture.Create<Category>(),
                StockLevel = fixture.Create<int>(),
                Brand = fixture.Create<string>()
            };

            // Create a basket and add a basket item for the product
            var basket = new Basket
            {
                CustomerId = Guid.NewGuid(),
                Status = BasketStatus.Completed // Assume basket is completed for sale generation
            };

            var basketItem = new BasketItem
            {
                Product = product,
                Quantity = fixture.Create<int>()
            };

            basket.Items.Add(basketItem);

            // Create the sale, linked to the basket
            var sale = new Sale
            {
                CustomerId = basket.CustomerId,
                SaleDate = fixture.Create<DateTime>(),
                Status = "Ordered",
                TotalSaleAmount = product.Price * basketItem.Quantity,
                Basket = basket
            };

            if (save)
            {
                dbContext.Products.Add(product);
                dbContext.Baskets.Add(basket);
                dbContext.Sales.Add(sale);
                await dbContext.SaveChangesAsync();
            }

            return sale;
        }

        private class TestProductsContext : ProductsContext
        {
            public TestProductsContext(DbContextOptions<ProductsContext> options) : base(options)
            {
            }

            public override void Dispose()
            {
            }

            public override ValueTask DisposeAsync()
            {
                return new ValueTask(Task.CompletedTask);
            }
        }

    }
}