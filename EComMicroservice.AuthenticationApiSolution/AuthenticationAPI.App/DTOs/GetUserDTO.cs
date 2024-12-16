using System.ComponentModel.DataAnnotations;

namespace AuthenticationAPI.App.DTOs;

public record GetUserDTO(
    int Id,
    [Required] string Name,
    [Required] string TelephoneNumber,
    [Required] string Address,
    [Required, EmailAddress] string Email,
    [Required] string Role
    );
