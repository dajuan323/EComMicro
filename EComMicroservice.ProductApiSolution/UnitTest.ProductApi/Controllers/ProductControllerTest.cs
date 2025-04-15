using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ProductApi.Controllers;

public class ProductControllerTest
{
    private readonly IProduct productInterface;
    private readonly ProductsController productsController;

    public ProductControllerTest()
    {
        // Set up dependencies
        productInterface = A.Fake<IProduct>();

        // Set up System Under Test = SUT
        productsController = new ProductsController(productInterface);
    }

    // GET ALL PRODUCTS
    [Fact]
    public async Task GetProduct_WhenProductExists_ReturnOkResponseWithProduct()
    {
        // Arrange
        var products = new List<Product>()
        {
            new() { Id = 1, Name = "Product 1", Quantity = 10, Price = 100.70m },
            new() { Id = 2, Name = "Product 2", Quantity = 123, Price = 233.34m },

        };

        // set up fake response for GetAllAsync
        A.CallTo(()=> productInterface.GetAllAsync()).Returns(products);

        // Act
        var result = await productsController.GetProducts();

        // Assert
        var okayResult = result.Result as OkObjectResult;
        okayResult.Should().NotBeNull();
        okayResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var returnedProducts = okayResult.Value as IEnumerable<ProductDTO>;
        returnedProducts.Should().NotBeNull();
        returnedProducts.Should().HaveCount(2);
        returnedProducts.First().Id.Should().Be(1);
        returnedProducts.Last().Id.Should().Be(2);


    }
}
