using System.ComponentModel.DataAnnotations;

namespace OrderApi.App.DTOs;

public record OrderDTO(
    int Id,
    [Required, Range(1, int.MaxValue)] int ClientId,
    [Required, Range(1, int.MaxValue)] int ProductId,
    [Required, Range(1, int.MaxValue)] int PurchaseQuantity,
    [DataType(DataType.DateTime)] DateTime OrderDate
    );
