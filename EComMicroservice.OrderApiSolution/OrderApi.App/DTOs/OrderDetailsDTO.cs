using System.ComponentModel.DataAnnotations;

namespace OrderApi.App.DTOs;

public record OrderDetailsDTO
(
    [Required] int OrderId,
    [Required] int ProductId,
    [Required] int ClientId,
    [Required] string ClientName,
    [Required, EmailAddress] string Email,
    [Required] string Address,
    [Required, Phone] string TelephoneNumber,
    [Required] string ProductName,
    [Required] int PurchaseQuantity,
    [Required, DataType(DataType.Currency)] decimal UnitPrice,
    [Required, DataType(DataType.Currency)] decimal Total,
    [Required, DataType(DataType.DateTime)] DateTime OrderDate

    );
