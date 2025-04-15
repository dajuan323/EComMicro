using OrderApi.App.DTOs;
using OrderApi.App.DTOs.Conversions;
using OrderApi.App.Interfaces;
using Polly;
using Polly.Registry;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace OrderApi.App.Services;

public class OrderService(IOrder orderInterface, HttpClient http,
    ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
{
    // Get Product
    public async Task<ProductDTO> GetProduct(int productId)
    {
        // Call API
        // Redirect call to the API Gateway since product API does not respond to outsiders
        var getProduct = await http.GetAsync($"/api/products/{productId}");
        if (!getProduct.IsSuccessStatusCode)
            return null!;

        var product =  await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
        return product!;
    }

    // GET USER
    public async Task<AppUserDTO> GetUser(int userId)
    {
        // Call API
        // Redirect call to the API Gateway since product API does not respond to outsiders
        var getUser = await http.GetAsync($"api/Authentication/{userId}");
        if (!getUser.IsSuccessStatusCode)
            return null!;

        var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
        return user!;
    }

    // GET ORDER DETAILS BY ID
    public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
    {
        // Prepare Order
        var order = await orderInterface.FindByIdAsync(orderId);
        if (order is null || order!.Id <= 0)
            return null!;

        // Get retry pipeline
        var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

        // Prepare product
        var productDTO = await retryPipeline.ExecuteAsync(async token =>
            await GetProduct(order.ProductId));

        // Prepare Client
        var appUserDTO = await retryPipeline.ExecuteAsync(async token =>
            await GetUser(order.ClientId));

        // Populate order Details
        return new OrderDetailsDTO(
            order.Id,
            productDTO.Id,
            appUserDTO.Id,
            appUserDTO.Name,
            appUserDTO.Email,
            appUserDTO.Address,
            appUserDTO.TelephoneNumber,
            productDTO.Name,
            order.PurchaseQuantity,
            productDTO.Price,
            productDTO.Quantity * order.PurchaseQuantity,
            order.OrderDate
            );
    }

    // GET ORDER BY CLIENT ID
    public async Task<IEnumerable<OrderDTO>> GetOrderByClientId(int clientId)
    {
        // Get all Client's orders
        var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
        if (!orders.Any()) return null!;

        // convert from Entity to DTO
        var (_, _orders) = OrderConversion.FromEntity(null, orders);
        return _orders!;
    }


}
