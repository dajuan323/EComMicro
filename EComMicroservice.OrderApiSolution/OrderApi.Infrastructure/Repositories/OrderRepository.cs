using EComMicro.SharedLibrary.Logs;
using EComMicro.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.App.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext context) : IOrder
{
    public async Task<Response> CreateAsync(Order entity)
    {
        try
        {
            var order = context.Orders.Add(entity).Entity;
            await context.SaveChangesAsync();
            return order.Id > 0 ? new Response(true, "Order placed successfully")
                : new Response(false, "Error placing order.");
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);

            // Display scary-free message to client
            return new Response(false, "Error placing order.");
        }
    }

    public async Task<Response> DeleteAsync(Order entity)
    {
        try
        {
            var order = await FindByIdAsync(entity.Id);
            if (order == null)
                return new Response(false, "Order not found.");

            context.Orders.Remove(order);
            await context.SaveChangesAsync();
            return new Response(true, "Order removed.");
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);

            // Display scary-free message to client
            return new Response(false, "Error removing order.");
        }
    }

    public async Task<Order> FindByIdAsync(int id)
    {
        try
        {
            var order = await context.Orders.FindAsync(id);
            return order ?? null!;
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);

            // Display scary-free message to client
            throw new Exception($"Error retrieving order with ID: [ {id} ].");
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        try
        {
            var orders = await context.Orders.AsNoTracking().ToListAsync();
            return orders ?? Enumerable.Empty<Order>();
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);

            // Display scary-free message to client
            throw new Exception("Error retrieving orders.");
        }
    }

    public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var order = await context.Orders.Where(predicate).FirstOrDefaultAsync()!;
            return order ?? null!;
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);

            // Display scary-free message to client
            throw new Exception("Error retrieving order.");
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var orders = await context.Orders.Where(predicate).ToListAsync();
            return orders ?? Enumerable.Empty<Order>();
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);

            // Display scary-free message to client
            throw new Exception("Error retrieving orders.");
        }
    }

    public async Task<Response> UpdateAsync(Order entity)
    {
        try
        {
            var order = await FindByIdAsync(entity.Id);
            if (order == null) 
                return new Response(false, $"Order with Id: {entity.Id} not found.");

            context.Entry(order).State = EntityState.Detached;
            context.Orders.Update(order);
            await context.SaveChangesAsync();
            return new Response(true, "Order updated.");
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);

            // Display scary-free message to client
            return new Response(false, "Error updating order.");
        }
    }
}
