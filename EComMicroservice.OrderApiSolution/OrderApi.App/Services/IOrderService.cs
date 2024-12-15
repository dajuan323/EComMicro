using OrderApi.App.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.App.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDTO>> GetOrderByClientId(int clientId);
    Task<OrderDetailsDTO> GetOrderDetails(int orderId);
}
