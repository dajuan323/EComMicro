using AuthenticationAPI.App.DTOs;
using AuthenticationAPI.App.Interfaces;
using AuthenticationAPI.Domain.Entities;
using AuthenticationAPI.Infrastructure.Data;
using EComMicro.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationAPI.Infrastructure.Repositories;

public class UserRepository(AuthenticationDbContext context, IUser userInterface, IConfiguration config) : IUser
{
    private IConfiguration _config = config;
    private async Task<AppUser> GetUserByEmail(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user ?? null!;
    }

    public async Task<GetUserDTO> GetUser(int userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return user != null ?
            new GetUserDTO(
                user.Id,
                user.Name!,
                user.TelephoneNumber!,
                user.Address!,
                user.Email!,
                user.Role!
                ) : null!;
    }

    public async Task<Response> Login(LoginDTO loginDTO)
    {
        var getUser = await GetUserByEmail(loginDTO.Email);
        if (getUser == null) return new Response(false, "Invalid credentials");

        bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
        if (!verifyPassword) return new Response(false, "Invalid credentials");

        string token = GenerateToken(getUser);
        return new Response(true, token);
    }

    private string GenerateToken(AppUser user)
    {
        var key = Encoding.UTF8.GetBytes(_config.GetSection("Authentication:Key").Value!);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name!),
            new(ClaimTypes.Email, user.Email!),
            new (ClaimTypes.Role, user.Role!)
        };
        if (!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
            claims.Add(new(ClaimTypes.Role, user.Role!));

        var token = new JwtSecurityToken(
            issuer: _config["Authentication:Issuer"],
            audience: _config["Authentication:Audience"],
            claims: claims,
            expires: null,
            signingCredentials: credentials,
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Response> Register(AppUserDTO appUserDTO)
    {
        var getUser = await GetUserByEmail(appUserDTO.Email);
        if (getUser == null) return new Response(false, $"Email ( {appUserDTO.Email} ) may not be used for registration.");

        var result = context.Users.Add(new AppUser()
        {
            Name = appUserDTO.Name,
            Email = appUserDTO.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
            TelephoneNumber = appUserDTO.TelephoneNumber,
            Address = appUserDTO.Address,
            Role = appUserDTO.Role,
        });

        await context.SaveChangesAsync();
        return result.Entity.Id > 0 ?
            new Response(true, "User registered successfully!")
            : new Response(false, "Invalid data provided.");
    }
}
