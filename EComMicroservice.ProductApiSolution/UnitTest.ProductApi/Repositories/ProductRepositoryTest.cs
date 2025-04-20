using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ProductApi.Repositories;

public class ProductRepositoryTest
{
    private readonly ProductDbContext _productDbContext;
    private readonly ProductRepository _productRepository;

    public ProductRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: "ProductDb").Options;

        _productDbContext = new ProductDbContext(options);
        _productRepository = new ProductRepository(_productDbContext);
    }

    // CREATE PRODUCT
    [Fact]
    public async Task CreateAsync_WhenProductAlreadyExists_ReturnErrorResponse()
    {
        // Arrange
        var exisitingProduct = new Product { Name = "ExistingProduct" };
        _productDbContext.Products.Add(exisitingProduct);
        await _productDbContext.SaveChangesAsync();

        // Act
        var result = await _productRepository.CreateAsync(exisitingProduct);

        // Assert
        result.Should().NotBeNull();
        result.Flag.Should().BeFalse();
        result.Message.Should().Be("ExistingProduct already exists.");
    }
}
