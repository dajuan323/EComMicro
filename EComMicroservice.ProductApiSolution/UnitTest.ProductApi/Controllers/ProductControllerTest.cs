using EComMicro.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

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
        List<Product>? products =
        [
            new() { Id = 1, Name = "Product 1", Quantity = 10, Price = 100.70m },
            new() { Id = 2, Name = "Product 2", Quantity = 123, Price = 233.34m },

        ];

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

    [Fact]
    public async Task GetProducts_WhenNoProductsExist_ReturnNotFoundResponse()
    {
        // Arrange
        List<Product>? products = new();

        // Set up fake response for GetAllAsync();
        A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

        // Act
        var result = await productsController.GetProducts();

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;

        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var message = notFoundResult.Value as string;
        message.Should().Be("No products detected in database.");
    }

    // GET PRODUCT BY ID
    [Fact]
    public async Task GetProductById_WhenProductExists_ReturnOkResultWithProduct()
    {
        // Arrange
        Product product = new() { Id = 1, Name = "Product 1", Quantity = 10, Price = 100.70m };

        // Set up fake response for GetById
        A.CallTo(() => productInterface.FindByIdAsync(1)).Returns(product);

        // Act
        var result = await productsController.GetProduct(1);

        // Assert
        var okayResult = result.Result as OkObjectResult;
        okayResult.Should().NotBeNull();
        okayResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var returnedProduct = okayResult.Value as ProductDTO;
        returnedProduct.Should().NotBeNull();
        returnedProduct.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetProductById_WhenProductDoesNotExist_ReturnNotFoundResponse()
    {
        // Arrange
        Product product = new();

        // Set up fake response for GetById
        A.CallTo(() => productInterface.FindByIdAsync(1)).Returns(value: (Product?)null);

        // Act
        var result = await productsController.GetProduct(1);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;

        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var message = notFoundResult.Value as string;
        message.Should().Be("Product not found");
    }

    // CREATE
    [Fact]
    public async Task CreateProduct_WhenModelStateIsInvalid_ReturnBadRequest()
    {
        // Arrange
        ProductDTO? productDto = new (1, "Product 1", 22, 67.90m);
        productsController.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await productsController.CreateProduct(productDto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CreateProduct_WhenCreatedSuccessfully_ReturnOkResponse()
    {
        // Arrange
        ProductDTO? productDto = new (1, "Baking Powder", 44, 3.56m);
        Response? response = new (true, "Created");

        // Act
        A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
        ActionResult<Response>? result = await productsController.CreateProduct(productDto);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        Response? responseResult = okResult.Value as Response;
        responseResult!.Message.Should().Be("Created");
        responseResult!.Flag.Should().BeTrue();
    }

    [Fact]
    public async Task CreateProduct_WhenCreateFails_ReturnBadRequestResponse()
    {
        // Arrange
        ProductDTO productDTO = new (1, "Product 1", 44, 32.43m);
        var response = new Response(false, "Failed");

        // Act
        A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
        var result = await productsController.CreateProduct(productDTO);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var responseResult = badRequestResult.Value as Response;
        responseResult.Should().NotBeNull();
        responseResult!.Message.Should().Be("Failed");
        responseResult!.Flag.Should().BeFalse();
    }

    // UPDATE
    [Fact]
    public async Task UpdateProduct_WhenUpdateIsSuccessful_ReturnOkResponse()
    {
        // Arrange
        ProductDTO productDTO = new(1, "Product 1", 44, 32.43m);
        var response = new Response(true, "Updated");

        // Act
        A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
        var result = await productsController.UpdateProduct(productDTO);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult!.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseResult = okResult.Value as Response;
        responseResult!.Message.Should().Be("Updated");
        responseResult!.Flag.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateProduct_WhenUpdateFails_ReturnBadRequestResponse()
    {
        // Arrange
        var productDTO = new ProductDTO(1, "Product 1", 70, 22.45m);
        var response = new Response(false, "Update Failed");

        // Act
        A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
        var result = await productsController.UpdateProduct(productDTO);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var responseResult = badRequestResult.Value as Response;
        responseResult.Should().NotBeNull();
        responseResult!.Message.Should().Be("Update Failed");
        responseResult!.Flag.Should().BeFalse();
    }

    // DELETE
    [Fact]
    public async Task DeleteProduct_WhenDeleteIsSuccessful_ReturnOkResponse()
    {
        // Arrange
        var productDTO = new ProductDTO(1, "Product 1", 25, 33.55m);
        var response = new Response(true, "Deleted successfully");

        // Set up to get fake response from DeleteAsync()
        A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

        // Act
        var result = await productsController.DeleteProduct(productDTO);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult!.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseResult = okResult.Value as Response;
        responseResult!.Message.Should().Be("Deleted successfully");
        responseResult!.Flag.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteProduct_WhenDeleteFails_ReturnBadRequestResponse()
    {
        // Arrange
        var productDTO = new ProductDTO(1, "Product 1", 25, 33.55m);
        var response = new Response(false, "Delete Failed.");

        // Set up to get fake response from DeleteAsync()
        A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

        // Act
        var result = await productsController.DeleteProduct(productDTO);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var responseResult = badRequestResult.Value as Response;
        responseResult.Should().NotBeNull();
        responseResult!.Message.Should().Be("Delete Failed.");
        responseResult!.Flag.Should().BeFalse();
    }
}
