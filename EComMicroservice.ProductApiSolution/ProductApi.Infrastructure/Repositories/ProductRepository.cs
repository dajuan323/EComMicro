using EComMicro.SharedLibrary.Logs;
using EComMicro.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories;

public class ProductRepository(ProductDbContext dbContext) : IProduct
{
    /// <summary>
    /// Creates instance of Product
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<Response> CreateAsync(Product entity)
    {
        try
        {
            // check if product already exists
            Product? getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
            if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                return new Response(false, $"{entity.Name} already exists.");

            var currentEntity = dbContext.Products.Add(entity).Entity;
            await dbContext.SaveChangesAsync();
            if (currentEntity is not null && currentEntity.Id > 0)
                return new Response(true, $"{entity.Name} added successfully.");

            else
                return new Response(false, $"Error occurred while adding {entity.Name}");
        }
        catch (Exception ex)
        {
            // Log original exception
            LogException.LogExceptions(ex);

            // Display scary-free message to client
            return new Response(false, "Error occurred adding new product.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>

    public async Task<Response> DeleteAsync(Product entity)
    {
        try
        {
            var product = await FindByIdAsync(entity.Id);
            if (product is null)
                return new Response(false, $"{entity.Name} not found");

            dbContext.Products.Remove(entity);
            await dbContext.SaveChangesAsync();
            return new Response(true, $"{entity.Name} deleted successfully.");
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);

            // Display scary-free message to client
            return new Response(false, "Error occurred deleting product.");

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Product> FindByIdAsync(int id)
    {
        try
        {
            Product? product = await dbContext.Products.FindAsync(id);
            return product is not null ? product : null!;
        }
        catch (Exception ex)
        {

            LogException.LogExceptions(ex);

            // Display scary-free message to client
            throw new Exception("Error occurred retrieving product.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            List<Product> products = await dbContext.Products.AsNoTracking().ToListAsync();
            return products is null ? null! : products;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            // Display scary-free message
            throw new InvalidOperationException($"Error occurred retrieving products.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
    {
        try
        {
            Product? product = await dbContext.Products.Where(predicate).FirstOrDefaultAsync()!;

            return product is not null ? product : null!;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            // Display scary-free message
            throw new InvalidOperationException($"Error occurred retrieving product.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<Response> UpdateAsync(Product entity)
    {
        try
        {
            Product? product = await FindByIdAsync(entity.Id);
            if (product is null)
                return new Response(false, $"{entity.Id} not found.");

            dbContext.Entry(product).State = EntityState.Detached;
            dbContext.Products.Update(entity);
            await dbContext.SaveChangesAsync();
            return new Response(true, $"{entity.Name} is updated successfully.");
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            // Display scary-free message
            return new Response(false, $"Error occurred updating product.");
        }
    }
}
