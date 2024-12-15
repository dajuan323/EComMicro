using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace OrderApi.App.DTOs;

public record ProductDTO(
    int Id,
    [Required] string Name,
    [Required, Range(1, int.MaxValue)] int Quantity,
    [Required, DataType(DataType.Currency)] decimal Price
    );
