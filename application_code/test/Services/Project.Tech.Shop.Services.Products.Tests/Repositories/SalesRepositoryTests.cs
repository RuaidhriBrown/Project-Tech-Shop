using FluentAssertions;
using Project.Tech.Shop.Tests.Common;
using Project.Tech.Shop.Services.Products.Enitites;
using Project.Tech.Shop.Services.Products.Repositories;
using AutoFixture;

namespace Project.Tech.Shop.Services.Products.Tests.Repositories;

public class SalesRepositoryTests
{
    private readonly ProductsContext _productsContext;
    private readonly SalesRepository _sut;

    public SalesRepositoryTests()
    {
        _productsContext = TestProductsContextDatabaseFactory.CreateDefaultTestContext();
        _sut = new SalesRepository(_productsContext);
    }

    [Theory, AutoMoqData]
    public async Task ShouldReturnAllSales(List<Sale> sales)
    {
        //arrange
        _productsContext.Sales.AddRange(sales);
        await _productsContext.SaveEntitiesAsync(CancellationToken.None);

        var saleToCheck = sales[0];

        //act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Count().Should().Be(sales.Count());
        result.Value.First().Should().Be(saleToCheck);
    }

    [Theory, AutoMoqData]
    public async Task ShouldReturnFailure_WhenSaleNotFound(Sale saleToCheck)
    {
        //act
        var result = await _sut.GetByIdAsync(saleToCheck.SaleId!, CancellationToken.None);

        //assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory, AutoMoqData]
    public async Task ShouldAddSaleSuccessfully(Sale sale)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var addResult = await _sut.AddAsync(sale, cancellationToken);
        var result = await _sut.GetByIdAsync(sale.SaleId, cancellationToken);

        // Assert
        addResult.IsSuccess.Should().BeTrue("because the product should be added successfully");
        result.IsSuccess.Should().BeTrue("because the product should be retrievable after being added");
        result.Value.Should().BeEquivalentTo(sale, "because the retrieved product should match the added product");
    }

    [Theory, AutoMoqData]
    public async Task ShouldUpdateSaleSuccessfully(Sale sale)
    {
        // Arrange
        await _sut.AddAsync(sale, CancellationToken.None);
        var saleId = Guid.NewGuid();
        sale.BasketId = saleId;

        // Act
        var updateResult = await _sut.UpdateAsync(sale, CancellationToken.None);
        var updatedSale = await _sut.GetByIdAsync(sale.SaleId, CancellationToken.None);

        // Assert
        updateResult.IsSuccess.Should().BeTrue("because the update should succeed");
        updatedSale.Value.BasketId.Should().Be(saleId, "because the product name should be updated");
    }

    [Theory, AutoMoqData]
    public async Task ShouldDeleteSaleSuccessfully(Sale sale)
    {
        // Arrange
        await _sut.AddAsync(sale, CancellationToken.None);

        // Act
        var deleteResult = await _sut.DeleteAsync(sale.SaleId, CancellationToken.None);
        var result = await _sut.GetByIdAsync(sale.SaleId, CancellationToken.None);

        // Assert
        deleteResult.IsSuccess.Should().BeTrue("because the deletion should succeed");
        result.IsFailure.Should().BeTrue("because the product should no longer exist");
    }

}
