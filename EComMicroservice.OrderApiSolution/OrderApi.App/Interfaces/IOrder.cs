using EComMicro.SharedLibrary.Interfaces;
using OrderApi.Domain.Entities;
using System.Linq.Expressions;

namespace OrderApi.App.Interfaces;

public interface IOrder : IGenericInterface<Order> 
{
    Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate);
}
