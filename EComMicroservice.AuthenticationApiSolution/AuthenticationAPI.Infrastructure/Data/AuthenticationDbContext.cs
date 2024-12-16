using AuthenticationAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPI.Infrastructure.Data;

public class AuthenticationDbContext(
    DbContextOptions<AuthenticationDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
}
