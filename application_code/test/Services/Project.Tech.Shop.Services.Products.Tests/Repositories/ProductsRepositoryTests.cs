using FluentAssertions;
using Project.Tech.Shop.Tests.Common;
using Project.Tech.Shop.Services.Products.Enitites;
using Project.Tech.Shop.Services.Products.Repositories;

namespace Project.Tech.Shop.Services.Products.Tests.Repositories;

public class ProductsRepositoryTests
{
    private readonly ProductsContext _productsContext;
    private readonly ProductsRepository _sut;

    public ProductsRepositoryTests()
    {
        _productsContext = TestProductsContextDatabaseFactory.CreateDefaultTestContext();
        _sut = new ProductsRepository(_productsContext);
    }

    [Theory, AutoMoqData]
    public async Task ShouldReturnAllProducts(List<Product> products)
    {
        //arrange
        _productsContext.Products.AddRange(products);
        await _productsContext.SaveEntitiesAsync(CancellationToken.None);

        var productToCheck = products[0];

        //act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Count().Should().Be(products.Count());
        result.Value.First().Should().Be(productToCheck);
    }

    [Theory, AutoMoqData]
    public async Task ShouldReturnMaybeNone_WhenProductNotFound(Product productToCheck)
    {
        //act
        var result = await _sut.GetByIdAsync(productToCheck.ProductId!, CancellationToken.None);

        //assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory, AutoMoqData]
    public async Task ShouldAddProductSuccessfully(Product product)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var addResult = await _sut.AddAsync(product, cancellationToken);
        var result = await _sut.GetByIdAsync(product.ProductId, cancellationToken);

        // Assert
        addResult.IsSuccess.Should().BeTrue("because the product should be added successfully");
        result.IsSuccess.Should().BeTrue("because the product should be retrievable after being added");
        result.Value.Should().BeEquivalentTo(product, "because the retrieved product should match the added product");
    }

    [Theory, AutoMoqData]
    public async Task ShouldUpdateProductSuccessfully(Product product)
    {
        // Arrange
        await _sut.AddAsync(product, CancellationToken.None);
        product.Name = "Updated Name";

        // Act
        var updateResult = await _sut.UpdateAsync(product, CancellationToken.None);
        var updatedProduct = await _sut.GetByIdAsync(product.ProductId, CancellationToken.None);

        // Assert
        updateResult.IsSuccess.Should().BeTrue("because the update should succeed");
        updatedProduct.Value.Name.Should().Be("Updated Name", "because the product name should be updated");
    }

    [Theory, AutoMoqData]
    public async Task ShouldDeleteProductSuccessfully(Product product)
    {
        // Arrange
        await _sut.AddAsync(product, CancellationToken.None);

        // Act
        var deleteResult = await _sut.DeleteAsync(product.ProductId, CancellationToken.None);
        var result = await _sut.GetByIdAsync(product.ProductId, CancellationToken.None);

        // Assert
        deleteResult.IsSuccess.Should().BeTrue("because the deletion should succeed");
        result.IsFailure.Should().BeTrue("because the product should no longer exist");
    }

    [Theory, AutoMoqData]
    public async Task ShouldReturnProductsByCategory(List<Product> products)
    {
        // Arrange
        foreach (var product in products)
        {
            await _sut.AddAsync(product, CancellationToken.None);
        }
        var expectedCategory = products[0].Category;

        // Act
        var result = await _sut.ListByCategoryAsync((Category)expectedCategory, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.All(p => p.Category == expectedCategory).Should().BeTrue("because all products should match the filtered category");
        result.Value.Should().ContainEquivalentOf(products[0], "because it includes at least one product from the setup");
    }

}
